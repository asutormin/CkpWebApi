using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpModel.Input;
using CkpServices.Helpers;
using CkpServices.Helpers.Factories;
using CkpServices.Helpers.Factories.Interfaces;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors
{
    class OrderProcesor : IOrderProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;
        private readonly string _basketOrderDescription;
        private readonly int _orderBusinessUnitId;
        private readonly int _defaultOrderManagerId;
        private readonly IBasketOrderFactory _basketOrderFactory;
        private readonly IClientOrderFactory _clientOrderFactory;

        public OrderProcesor(
            BPFinanceContext context,
            IBPFinanceRepository repository,
            string basketOrderDescription,
            int orderBusinessUnitId,
            int defaultOrderManagerId)
        {
            _context = context;
            _repository = repository;

            _basketOrderDescription = basketOrderDescription;
            _orderBusinessUnitId = orderBusinessUnitId;
            _defaultOrderManagerId = defaultOrderManagerId;

            _basketOrderFactory = new BasketOrderFactory(_orderBusinessUnitId, _basketOrderDescription);
            _clientOrderFactory = new ClientOrderFactory();
        }

        #region Get

        public Order GetOrderById(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.BusinessUnit)
                .Include(o => o.ClientLegalPerson.AccountSettings)
                .Include(o => o.OrderPositions).ThenInclude(op => op.Price)
                .Include(o => o.OrderPositions).ThenInclude(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Single(o => o.Id == orderId);

            return order;
        }

        public Order GetBasketOrder(int clientLegalPersonId)
        {
            // Ищем ПЗ заказ клиента с примечанием "Корзина_ЛК"
            var order = _context.Orders
                .Include(o => o.OrderPositions).ThenInclude(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Where(
                    o =>
                       o.ClientLegalPersonId == clientLegalPersonId &&
                       o.ActivityTypeId == 20 &&
                       o.Description == _basketOrderDescription)
                .FirstOrDefault();

            return order;
        }

        public IQueryable<OrderPosition> GetBasketOrderPositionsQuery(int clientLegalPersonId)
        {
            var query = _context.OrderPositions
                .Include(op => op.Order.AccountOrder)
                .Include(op => op.ParentOrderPosition)
                .Include(op => op.Price)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.RubricPositions).ThenInclude(rp => rp.Rubric)
                .Where(
                    op =>
                        op.ParentOrderPositionId == null &&
                        op.Order.ClientLegalPersonId == clientLegalPersonId &&
                        op.Order.ActivityTypeId == 20 &&
                        op.Order.Description == _basketOrderDescription &&
                        op.Order.BusinessUnitId == _orderBusinessUnitId);

            return query;
        }

        #endregion

        #region Create

        public Order CreateBasketOrder(OrderPositionData opd, DbTransaction dbTran)
        {
            var clientLegalPersonId = opd.ClientLegalPersonId;

            var clientCompanyId = _context.LegalPersons
                .Include(lp => lp.Company)
                .Single(lp => lp.Id == opd.ClientLegalPersonId).Company.Id;

            var supplierLegalPersonId = _context.BusinessUnits
                .Single(bu => bu.Id == _orderBusinessUnitId).LegalPersonId;

            var maxExitDate = _context.Graphics
                .Where(gr => opd.GetGraphicsWithChildren()
                .Contains(gr.Id))
                .Max(gr => gr.OutDate);

            var sum = _context.Prices
                .Where(p => opd.GetPrices().Contains(p.Id))
                .AsEnumerable()
                .Sum(p => p.GetTarifCost());

            var businessUnitCompanyManager = _context.BusinessUnitCompanyManagers
                .SingleOrDefault(
                    bucm =>
                        bucm.CompanyId == clientCompanyId &&
                        bucm.BusinessUnitId == _orderBusinessUnitId);

            var managerId = businessUnitCompanyManager == null
                ? _defaultOrderManagerId
                : businessUnitCompanyManager.ManagerId;

            var order = _basketOrderFactory.Create(
                clientLegalPersonId, clientCompanyId, supplierLegalPersonId, maxExitDate, sum, managerId);

            order = _repository.SetOrder(order, true, dbTran);

            return order;
        }

        public Order CreateClientOrder(Order basketOrder, IEnumerable<OrderPosition> orderPositions, DbTransaction dbTran)
        {
            var order = _clientOrderFactory.Create(basketOrder, orderPositions);
            order = _repository.SetOrder(order, true, dbTran);

            return order;
        }

        #endregion

        #region Update

        public void UpdateOrder(int orderId, DbTransaction dbTran)
        {
            var order = GetOrderById(orderId);

            RefreshOrder(order, dbTran);
        }

        #endregion

        #region Refresh

        public void RefreshOrder(Order order, DbTransaction dbTran)
        {
            var sum = order.OrderPositions.GetClientSum();
            var maxExitDate = order.OrderPositions.GetMaxExitDate();

            if (order.Sum == sum && order.MaxExitDate == maxExitDate)
                return;

            order.Sum = sum;
            order.MaxExitDate = maxExitDate;

            _repository.SetOrder(order, true, dbTran);
            _context.Entry(order).Reload();
        }

        #endregion
    }
}
