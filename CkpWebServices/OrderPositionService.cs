using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using CkpDAL;
using CkpServices.Interfaces;
using CkpModel.Output;
using CkpModel.Input;
using CkpInfrastructure.Configuration;
using CkpServices.Helpers;
using CkpServices.Processors.Interfaces;
using CkpServices.Processors;
using CkpDAL.Repository;
using CkpServices.Processors.String;
using CkpServices.Helpers.Providers;

namespace CkpServices
{
    public partial class OrderPositionService : IOrderPositionService
    {
        private readonly BPFinanceContext _context;

        private readonly IOrderPositionDataProcessor _orderPositionDataProcessor;
        private readonly IClientProcessor _clientProcessor;
        private readonly IOrderProcessor _orderProcessor;
        private readonly IOrderPositionProcessor _orderPositionProcessor;

        public OrderPositionService(BPFinanceContext context, IOptions<AppSettings> appSettingsAccessor, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            var orderImFolderTemplate = appSettingsAccessor.Value.OrderImFolderTemplate;
            var dbName = appSettingsAccessor.Value.DatabaseName;

            var businessUnitIdByPriceIdProvider = new BusinessUnitIdByPriceIdProvider(_context);

             _orderPositionDataProcessor = new OrderPositionDataProcessor(
                _context,
                orderImFolderTemplate,
                dbName);
            _clientProcessor = new ClientProcessor(
                _context);
            _orderProcessor = new OrderProcesor(
                _context,
                repository,
                appParamsAccessor.Value.BasketOrderDescription,
                appParamsAccessor.Value.ManagerId,
                businessUnitIdByPriceIdProvider);
            var rubricProcessor = new RubricProcessor(
                _context,
                repository);
            var graphicProcessor = new GraphicProcessor(
                _context,
                repository);
            var orderImProcessor = new OrderImProcessor(
                _context,
                repository);
            var stringProcessor = new StringProcessor(
                _context,
                repository);
            var moduleProcessor = new ModuleProcessor(
                appSettingsAccessor.Value.OrderImFolderTemplate,
                appSettingsAccessor.Value.DatabaseName);
            var positionImProcessor = new PositionImProcessor(
                _context,
                repository,
                orderImProcessor,
                stringProcessor,
                moduleProcessor);
            _orderPositionProcessor = new OrderPositionProcessor(
                _context,
                repository,
                rubricProcessor,
                graphicProcessor,
                positionImProcessor,
                businessUnitIdByPriceIdProvider);
        }

        public bool ExistsById(int orderPositionId)
        {
            var orderPosition = _context.OrderPositions
                .SingleOrDefault(
                    op =>
                        op.Id == orderPositionId);

            return orderPosition != null;
        }

        #region Get

        public IEnumerable<OrderPositionInfo> GetBasket(int clientLegalPersonId)
        {
            var result = _orderProcessor
                .GetBasketOrderPositionsQuery(clientLegalPersonId)
                .SelectPositions();

            return result;
        }

        public async Task<OrderPositionData> GetOrderPositionDataAsync(int orderPositionId)
        {
            return await _orderPositionDataProcessor.GetOrderPositionFullDataAsync(orderPositionId);
        }

        #endregion

        #region Create
        public void CreateOrderPosition(OrderPositionData opd)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                // Получаем заказ-корзину
                var basketOrder = _orderProcessor.GetBasketOrder(opd.ClientLegalPersonId, opd.PriceId);

                // Если заказа-корзины не существует - создаём её
                if (basketOrder == null)
                    basketOrder = _orderProcessor.CreateBasketOrder(opd, dbTran);

                // Получааем скидку клиента
                var clientDiscount = _clientProcessor.GetDiscount(
                    opd.ClientLegalPersonId,
                    basketOrder.BusinessUnitId,
                    opd.SupplierId,
                    opd.FormatData.FormatTypeId);

                // Сохраняем позицию заказа
                int orderPositionId = _orderPositionProcessor.CreateFullOrderPosition(basketOrder.Id, null, clientDiscount, opd, dbTran);

                // Сохраняем пакетные позиции заказа
                foreach (var child in opd.Childs)
                    _orderPositionProcessor.CreateFullOrderPosition(basketOrder.Id, orderPositionId, clientDiscount, child, dbTran);

                // Обновляем заказ-корзину
                _orderProcessor.UpdateOrder(basketOrder.Id, dbTran);

                _context.SaveChanges();
                dbTran.Commit();
            }
        }

        #endregion

        #region Update
        public void UpdateOrderPosition(OrderPositionData opd)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                var orderPosition = _orderPositionProcessor.GetOrderPositionsByIds(new[] { opd.OrderPositionId }).Single();

                var basketOrder = _orderProcessor.GetBasketOrder(opd.ClientLegalPersonId, opd.PriceId);

                // Обновляем позицию заказа
                _orderPositionProcessor.UpdateFullOrderPosition(orderPosition, opd, dbTran);

                // Перебираем переданные пакетные позиции
                foreach (var child in opd.Childs)
                {
                    var childOrderPosition = _orderPositionProcessor.GetOrderPositionsByIds(new[] { child.OrderPositionId }).SingleOrDefault();

                    // Если переданной позиции не существует - создаём её, иначе - обновляем
                    if (_orderPositionProcessor.NeedCreateFullOrderPosition(childOrderPosition))
                        _orderPositionProcessor.CreateFullOrderPosition(basketOrder.Id, orderPosition.Id, 0, child, dbTran);
                    else
                        _orderPositionProcessor.UpdateFullOrderPosition(childOrderPosition, child, dbTran);
                }

                var childOrderPositions = orderPosition.ChildOrderPositions;

                // Если в существующих пакетных позициях нет переданных - удаляем их
                foreach (var childOrderPosition in childOrderPositions)
                    if (_orderPositionProcessor.NeedDeleteFullOrderPosition(childOrderPosition.Id, opd.Childs))
                        _orderPositionProcessor.DeleteFullOrderPosition(childOrderPosition, dbTran);

                // Обновляем заказ-корзину
                _orderProcessor.UpdateOrder(basketOrder.Id, dbTran);

                _context.SaveChanges();
                dbTran.Commit();
            }
        }

        #endregion

        #region Delete
        public void DeleteOrderPosition(int orderPositionId)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                var orderPosition = _orderPositionProcessor.GetOrderPositionsByIds(new[] { orderPositionId })
                    .SingleOrDefault();

                if (orderPosition == null)
                    return;

                var childOrderPositions = orderPosition.ChildOrderPositions
                    .ToList();

                for (int i = childOrderPositions.Count - 1; i >= 0; i--)
                    _orderPositionProcessor.DeleteFullOrderPosition(childOrderPositions[i], dbTran);

                _orderPositionProcessor.DeleteFullOrderPosition(orderPosition, dbTran);

                // Обновляем заказ-корзину
                _orderProcessor.UpdateOrder(orderPosition.Order.Id, dbTran);

                dbTran.Commit();
            }
        }

        #endregion
    }
}
