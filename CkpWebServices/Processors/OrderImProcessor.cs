using CkpDAL;
using CkpDAL.Model;
using CkpDAL.Repository;
using CkpServices.Helpers.Factories;
using CkpServices.Helpers.Factories.Interfaces;
using CkpServices.Processors.Interfaces;
using System;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors
{
    class OrderImProcessor : IOrderImProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IOrderImFactory _orderImFactory;

        public OrderImProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;

            _orderImFactory = new OrderImFactory();
        }

        #region Get

        public OrderIm GetOrderIm(int orderId, int orderImTypeId)
        {
            var orderIm = _context.OrderIms
                .SingleOrDefault(
                    oim =>
                        oim.OrderId == orderId &&
                        oim.OrderImType.Id == orderImTypeId);

            return orderIm;
        }

        #endregion

        #region Create

        public OrderIm CreateOrderIm(int orderId, int orderImTypeId, Action<OrderIm> processAction,  DbTransaction dbTran)
        {
            var orderIm = _orderImFactory.Create(orderId, orderImTypeId);

            if (processAction != null)
                processAction.Invoke(orderIm);

            orderIm = _repository.SetOrderIm(orderIm, isActual: true, dbTran);

            return orderIm;
        }

        #endregion

        #region Update

        public OrderIm UpdateOrderIm(OrderIm orderIm, DbTransaction dbTran)
        {
            orderIm = _repository.SetOrderIm(orderIm, isActual: true, dbTran);

            return orderIm;
        }

        #endregion

        #region Delete

        public void DeleteOrderIm(OrderIm orderIm, DbTransaction dbTran)
        {
            _repository.SetOrderIm(orderIm, isActual: false, dbTran);
            _context.Entry(orderIm).Reload();
        }

        public bool NeedDeleteOrderIm(OrderIm orderIm)
        {
            return !_context.PositionIms
                .Any(
                    pi =>
                        pi.OrderId == orderIm.OrderId &&
                        pi.PositionImType.OrderImTypeId == orderIm.OrderImTypeId);
        }

        #endregion

        #region Process

        public void ProcessOrderImStatus(OrderIm orderIm)
        {
            if (orderIm.OrderImTypeId == 1)
            {
                // Для строк меняем статус ИМ-а на "Готов"
                orderIm.MaketStatusId = 4;
            }
            else
            {
                // Для всех остальных (модули и пр.) меняем статус ИМ-а заказа на "Вёрстка"
                orderIm.MaketStatusId = 3;
            }
        }

        #endregion
    }
}
