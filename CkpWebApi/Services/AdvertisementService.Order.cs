using CkpWebApi.DAL.Model;
using CkpWebApi.Helpers;
using CkpWebApi.InputEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpWebApi.Services
{
    public partial class AdvertisementService
    {
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
                .Include(op => op.ChildOrderPositions)
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

        private Order GetShoppingCartOrder(int clientLegalPersonId)
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

        private int CreateShoppingCartOrder(Advertisement adv, DbTransaction dbTran)
        {
            var orderId = 0;
            var clientLegalPersonId = adv.ClientLegalPersonId;

            var clientCompanyId = _context.LegalPersons
                .Include(lp => lp.Company)
                .Single(lp => lp.Id == adv.ClientLegalPersonId).Company.Id;

            var supplierLegalPersonId = _context.BusinessUnits
                .Single(bu => bu.Id == _orderBusinessUnitId).LegalPersonId;

            var maxExitDate = _context.Graphics
                .Where(gr => adv.GetGraphicsWithChildren().Contains(gr.Id))
                .Max(gr => gr.OutDate);

            var orderSum = _context.Prices
                .Where(p => adv.GetPrices().Contains(p.Id))
                .AsEnumerable()
                .Sum(p => p.GetTarifCost());

            var lastEditDate = DateTime.Now;

            _repository.SetOrder(
                    dbTran: dbTran,
                    id: ref orderId,
                    parentId: null,
                    activityTypeId: 20,
                    businessUnitId: _orderBusinessUnitId,
                    statusId: 1, // Редактирование
                    visa: false,
                    isNeedAccount: false,
                    accountDescription: string.Empty,
                    orderNumber: string.Empty,
                    orderDate: DateTime.Now.Date,
                    maxExitDate: maxExitDate,
                    companyId: clientCompanyId,
                    clientLegalPersonId: clientLegalPersonId,
                    supplierLegalPersonId: supplierLegalPersonId,
                    orderSum: orderSum,
                    orderPaid: 0,
                    isCashless: false,
                    isAdvance: false,
                    isPaymentWithAgent: false,
                    isFactoring: false,
                    createdPaymentPrognosisTypeId: 1,
                    currentPaymentPrognosisTypeId: 1,
                    paymentArbitaryPrognosisDate: null,
                    description: _orderDescription,
                    request: string.Empty,
                    managerId: _managerId,
                    editUserId: _editUserId,
                    isActual: true,
                    lastEditDate: ref lastEditDate);

            return orderId;
        }

        private void SetNeedRefreshOrderShoppingCartOrder()
        {
            _needRefreshShoppingCartOrder = true;
        }

        private bool NeedRefreshOrder(Order order, float orderSum, DateTime? maxExitDate)
        {
            if (order.Sum == orderSum && order.MaxExitDate == maxExitDate)
                return false;

            return true;
        }

        private void RefreshOrder(int orderId, DbTransaction dbTran)
        {
            var order = _context.Orders
                .Include(o => o.OrderPositions).ThenInclude(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(o => o.OrderPositions).ThenInclude(op => op.Price)
                .Single(o => o.Id == orderId);
                      
            var orderSum = order.OrderPositions.GetClientSum();
            var maxExitDate = order.OrderPositions.GetMaxExitDate();

            if (!NeedRefreshOrder(order, orderSum, maxExitDate))
                return;

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
                maxExitDate: maxExitDate,
                companyId: order.ClientCompanyId,
                clientLegalPersonId: order.ClientLegalPersonId,
                supplierLegalPersonId: order.SupplierLegalPersonId,
                orderSum: orderSum,
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
                isActual: true,
                lastEditDate: ref lastEditDate);

            _context.Entry(order).Reload();
        }

        private void RefreshOrder(Order order, DbTransaction dbTran)
        {
            var orderId = order.Id;

            var orderSum = order.OrderPositions.GetClientSum();
            var maxExitDate = order.OrderPositions.GetMaxExitDate();

            if (!NeedRefreshOrder(order, orderSum, maxExitDate))
                return;

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
                maxExitDate: maxExitDate,
                companyId: order.ClientCompanyId,
                clientLegalPersonId: order.ClientLegalPersonId,
                supplierLegalPersonId: order.SupplierLegalPersonId,
                orderSum: orderSum,
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
                isActual: true,
                lastEditDate: ref lastEditDate);

            _context.Entry(order).Reload();
        }

        private int CreateOrder(Order shoppingCartOrder, IEnumerable<OrderPosition> orderPositions, DbTransaction dbTran)
        {
            var orderId = 0;

            var orderSum = orderPositions.GetClientSum();
            var maxExitDate = orderPositions.GetMaxExitDate();

            var lastEditDate = DateTime.Now;

            _repository.SetOrder(
                dbTran: dbTran,
                id: ref orderId,
                parentId: null,
                activityTypeId: 1,
                businessUnitId: shoppingCartOrder.BusinessUnitId,
                statusId: 0, // ---
                visa: false,
                isNeedAccount: true,
                accountDescription: string.Empty,
                orderNumber: string.Empty,
                orderDate: DateTime.Now.Date,
                maxExitDate: maxExitDate,
                companyId: shoppingCartOrder.ClientCompanyId,
                clientLegalPersonId: shoppingCartOrder.ClientLegalPersonId,
                supplierLegalPersonId: shoppingCartOrder.SupplierLegalPersonId,
                orderSum: orderSum,
                orderPaid: 0,
                isCashless: false,
                isAdvance: false,
                isPaymentWithAgent: false,
                isFactoring: false,
                createdPaymentPrognosisTypeId: 1,
                currentPaymentPrognosisTypeId: 1,
                paymentArbitaryPrognosisDate: null,
                description: string.Empty,
                request: string.Empty,
                managerId: _managerId,
                editUserId: _editUserId,
                isActual: true,
                lastEditDate: ref lastEditDate);

            return orderId;
        }
    }
}
