using CkpDAL;
using CkpDAL.Helpers;
using CkpDAL.Model;
using CkpEntities.Input;
using CkpServices.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices
{
    public partial class AdvertisementService
    {
        #region Get

        private Order GetOrderById(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.BusinessUnit)
                .Include(o  => o.ClientLegalPerson.AccountSettings)
                .Include(o => o.OrderPositions).ThenInclude(op => op.Price)
                .Include(o => o.OrderPositions).ThenInclude(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Single(o => o.Id == orderId);

            return order;
        }

        private IEnumerable<OrderPosition> GetOrderPositionsByIds(int[] orderPositionIds)
        {
            var orderPositions = _context.OrderPositions
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PositionIm.PositionImType.OrderImType)
                .Include(op => op.Supplier).ThenInclude(su => su.Company)
                .Include(op => op.Supplier).ThenInclude(su => su.City)
                .Include(op => op.Price)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.PricePositionType)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.Unit)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.RubricPositions)
                .Include(op => op.PositionIm.PositionImType.OrderImType)
                .Where(op => orderPositionIds.Contains(op.Id))
                .ToList();

            return orderPositions;
        }

        private Order GetShoppingCartOrderByClientLegalPersonId(int clientLegalPersonId)
        {
            // Ищем ПЗ заказ клиента с примечанием "Корзина_ЛК"
            var order = _context.Orders
                .Include(o => o.OrderPositions).ThenInclude(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Where(
                    o =>
                       o.ClientLegalPersonId == clientLegalPersonId &&
                       o.ActivityTypeId == 20 &&
                       o.Description == _orderDescription)
                .FirstOrDefault();

            return order;
        }

        #endregion

        #region Create

        private Order CreateShoppingCartOrder(Advertisement adv, DbTransaction dbTran)
        {
            var clientLegalPersonId = adv.ClientLegalPersonId;

            var clientCompanyId = _context.LegalPersons
                .Include(lp => lp.Company)
                .Single(lp => lp.Id == adv.ClientLegalPersonId).Company.Id;

            var supplierLegalPersonId = _context.BusinessUnits
                .Single(bu => bu.Id == _orderBusinessUnitId).LegalPersonId;

            var maxExitDate = _context.Graphics
                .Where(gr => adv.GetGraphicsWithChildren().Contains(gr.Id))
                .Max(gr => gr.OutDate);

            var sum = _context.Prices
                .Where(p => adv.GetPrices().Contains(p.Id))
                .AsEnumerable()
                .Sum(p => p.GetTarifCost());

            var businessUnitCompanyManager = _context.BusinessUnitCompanyManagers
                .SingleOrDefault(
                    bucm =>
                        bucm.CompanyId == clientCompanyId &&
                        bucm.BusinessUnitId == _orderBusinessUnitId);

            var managerId = businessUnitCompanyManager == null
                ? _managerId
                : businessUnitCompanyManager.ManagerId;

            var order = _shoppingCartOrderFactory.Create(
                clientLegalPersonId, clientCompanyId, supplierLegalPersonId, maxExitDate, sum, managerId);

            order = SetOrder(order, true, dbTran);

            return order;
        }

        private Order CreateClientOrder(Order shoppingCartOrder, IEnumerable<OrderPosition> orderPositions, DbTransaction dbTran)
        {
            // Создаём клиентский заказ
            var clientOrder = _clientOrderFactory.Create(shoppingCartOrder, orderPositions);
            clientOrder = SetOrder(clientOrder, true, dbTran);

            // Привязываем к новому заказу позиции из корзины
            var orderPositionsWithChildren = orderPositions
                .Union(
                    orderPositions.SelectMany(op => op.ChildOrderPositions));

            ChangeOrderPositionsOrder(orderPositionsWithChildren, clientOrder.Id, dbTran);

            // Привязываем к новому заказу позиции ИМ-ов
            var positionsIm = orderPositionsWithChildren
                .Where(op => op.PositionIm != null)
                .Select(op => op.PositionIm);

            ChangePositionImsOrder(positionsIm, clientOrder.Id, dbTran);

            var orderImTypeIds = orderPositions
                .SelectMany(op => op.ChildOrderPositions)
                .Where(op => op.PositionIm != null)
                .GroupBy(op => op.PositionIm.PositionImType.OrderImType.Id)
                .Select(g => g.Key)
                .ToList();

            foreach (var orderImTypeId in orderImTypeIds)
            {
                // Создаём ИМ нового заказа
                var orderIm = _orderImFactory.Create(clientOrder.Id, orderImTypeId);

                ProcessOrderImStatus(orderIm, dbTran,
                    (orderIm, isActual, dbTran) => SetOrderIm(orderIm, true, dbTran));

                // Если ИМ заказа корзины больше не нужен - удаляем его
                var shoppingCartOrderIm = GetOrderIm(shoppingCartOrder.Id, orderImTypeId);
                if (NeedDeleteOrderIm(shoppingCartOrderIm))
                    DeleteOrderIm(shoppingCartOrderIm, dbTran);
            }

            // Обновляем заказ-корзину
            RefreshOrder(shoppingCartOrder.Id, dbTran);

            return clientOrder;
        }

        #endregion

        #region Refresh

        private bool NeedRefreshOrder(Order order, float orderSum, DateTime? maxExitDate)
        {
            if (order.Sum == orderSum && order.MaxExitDate == maxExitDate)
                return false;

            return true;
        }

        private void RefreshOrder(int orderId, DbTransaction dbTran)
        {
            var order = GetOrderById(orderId);

            RefreshOrder(order, dbTran);
        }

        private void RefreshOrder(Order order, DbTransaction dbTran)
        {
            var sum = order.OrderPositions.GetClientSum();
            var maxExitDate = order.OrderPositions.GetMaxExitDate();

            if (!NeedRefreshOrder(order, sum, maxExitDate))
                return;

            order.Sum = sum;
            order.MaxExitDate = maxExitDate;

            SetOrder(order, true, dbTran);

            _context.Entry(order).Reload();
        }



        #endregion

        #region Set

        private Order SetOrder(Order order, bool isActual, DbTransaction dbTran)
        {
            var orderId = order.Id;
            var lastEditDate = order.BeginDate;

            _repository.SetOrder(
                dbTran: dbTran,
                id: ref orderId,
                parentId: order.ParentOrderId,
                activityTypeId: order.ActivityTypeId,
                businessUnitId: order.BusinessUnitId,
                statusId: order.StatusId,
                visa: order.IsNeedVisa,
                isNeedAccount: order.IsNeedAccount,
                accountDescription: order.AccountDescription,
                orderNumber: order.OrderNumber,
                orderDate: order.OrderDate,
                maxExitDate: order.MaxExitDate,
                companyId: order.ClientCompanyId,
                clientLegalPersonId: order.ClientLegalPersonId,
                supplierLegalPersonId: order.SupplierLegalPersonId,
                orderSum: order.Sum,
                orderPaid: order.Paid,
                isCashless: order.IsCashless,
                isAdvance: order.IsAdvance,
                isPaymentWithAgent: order.IsPaymentWithAgent,
                isFactoring: order.IsFactoring,
                createdPaymentPrognosisTypeId: order.CreatedPaymentPrognosisTypeId,
                currentPaymentPrognosisTypeId: order.CurrentPaymentPrognosisTypeId,
                paymentArbitaryPrognosisDate: order.PaymentArbitaryPrognosisDate,
                description: order.Description,
                request: order.Request,
                managerId: order.ManagerId,
                editUserId: _editUserId,
                isActual: isActual,
                lastEditDate: ref lastEditDate);

            order.Id = orderId;
            order.BeginDate = lastEditDate;

            return order;
        }

        #endregion
    }
}
