using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpInfrastructure.Providers.Interfaces;
using CkpModel.Input;
using CkpServices.Helpers;
using CkpServices.Helpers.Factories;
using CkpServices.Helpers.Factories.Interfaces;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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
        private readonly int _defaultOrderManagerId;
        private readonly IBasketOrderFactory _basketOrderFactory;
        private readonly IClientOrderFactory _clientOrderFactory;
        private readonly IKeyedProvider<Tuple<int, int>, int> _basketBusinessUnitIdProvider;

        public OrderProcesor(
            BPFinanceContext context,
            IBPFinanceRepository repository,
            string basketOrderDescription,
            int defaultOrderManagerId,
            IKeyedProvider<Tuple<int, int>, int> basketBusinessUnitIdProvider)
        {
            _context = context;
            _repository = repository;

            _basketOrderDescription = basketOrderDescription;
            _defaultOrderManagerId = defaultOrderManagerId;

            _basketBusinessUnitIdProvider = basketBusinessUnitIdProvider;

            _basketOrderFactory = new BasketOrderFactory(_basketOrderDescription);
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

        public Order GetBasketOrder(int clientLegalPersonId, int priceId)
        {
            // Ищем бизнес юнит заказа корзины
            var businessUnitId = _basketBusinessUnitIdProvider.GetByValue(
                new Tuple<int, int>(clientLegalPersonId, priceId));

            // Ищем ПЗ заказ клиента с примечанием "Корзина_ЛК"
            var order = _context.Orders
                .Include(o => o.OrderPositions).ThenInclude(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Where(
                    o =>
                       o.ClientLegalPersonId == clientLegalPersonId &&
                       o.ActivityTypeId == 20 &&
                       o.BusinessUnitId == businessUnitId &&
                       o.Description == _basketOrderDescription)
                .FirstOrDefault();

            return order;
        }

        #endregion

        #region Create

        public Order CreateBasketOrder(OrderPositionData opd, DbTransaction dbTran)
        {
            var clientLegalPersonId = opd.ClientLegalPersonId;

            // Ищем бизнес юнит заказа корзины
            var businessUnitId = _basketBusinessUnitIdProvider.GetByValue(
                new Tuple<int, int>(clientLegalPersonId, opd.PriceId));

            var clientCompanyId = _context.LegalPersons
                .Include(lp => lp.Company)
                .Single(lp => lp.Id == opd.ClientLegalPersonId).Company.Id;

            var supplierLegalPersonId = _context.BusinessUnits
                .Single(bu => bu.Id == businessUnitId).LegalPersonId;

            var isAdvance = _context.LegalPersons
                .Include(lp => lp.AccountSettings)
                .Single(lp => lp.Id == clientLegalPersonId)
                .AccountSettings.IsNeedPrepayment;

            var maxExitDate = _context.Graphics
                .Where(gr => opd.GetGraphicsWithChildren()
                .Contains(gr.Id))
                .Max(gr => gr.OutDate);

            var sum = _context.Prices
                .Where(p => opd.GetPrices().Contains(p.Id))
                .AsEnumerable()
                .Sum(p => p.GetTarifCost());

            var managerId = _defaultOrderManagerId;

            var order = _basketOrderFactory.Create(
                businessUnitId, clientLegalPersonId, clientCompanyId,
                supplierLegalPersonId, isAdvance, maxExitDate, sum, managerId);

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

        public void UpdateOrder(Order order, DbTransaction dbTran)
        {
            _repository.SetOrder(order, isActual: true, dbTran);
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

            _repository.SetOrder(order, isActual:true, dbTran);
            _context.Entry(order).Reload();
        }

        #endregion
    }
}
