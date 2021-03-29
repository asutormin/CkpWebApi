using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using CkpDAL;
using CkpServices.Interfaces;
using CkpEntities.Output;
using CkpEntities.Input;
using CkpEntities.Configuration;
using CkpServices.Helpers;
using CkpServices.Processors.Interfaces;
using CkpServices.Processors;
using CkpDAL.Repository;
using CkpServices.Processors.String;

namespace CkpServices
{
    public partial class AdvertisementService : IAdvertisementService
    {
        private readonly BPFinanceContext _context;

        private readonly IAdvertisementProcessor _advertisementProcessor;
        private readonly IClientProcessor _clientProcessor;
        private readonly IOrderProcessor _orderProcessor;
        private readonly IOrderPositionProcessor _orderPositionProcessor;

        public AdvertisementService(BPFinanceContext context, IOptions<AppSettings> appSettingsAccessor, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            var orderImFolderTemplate = appSettingsAccessor.Value.OrderImFolderTemplate;
            var dbName = appSettingsAccessor.Value.DatabaseName;
            var orderBusinessUnitId = appParamsAccessor.Value.OrderBusinessUnitId;

            _advertisementProcessor = new AdvertisementProcessor(
                _context,
                orderImFolderTemplate,
                dbName);
            _clientProcessor = new ClientProcessor(
                _context);
            _orderProcessor = new OrderProcesor(
                _context,
                repository,
                appParamsAccessor.Value.ShoppingCartOrderDescription,
                orderBusinessUnitId,
                appParamsAccessor.Value.ManagerId);
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
                orderBusinessUnitId);
        }

        #region Get

        public async Task<ActionResult<IEnumerable<PositionInfo>>> GetShoppingCartAsync(int clientLegalPersonId)
        {
            var result = await _orderProcessor
                .GetShoppingCartOrderPositionsQuery(clientLegalPersonId)
                .SelectPositions()
                .ToListAsync();

            return result;
        }

        public Advertisement GetAdvertisementFull(int orderPositionId)
        {
            return _advertisementProcessor.GetAdvertisementFull(orderPositionId);
        }

        #endregion

        #region Create
        public void CreateAdvertisement(Advertisement adv)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                // Получаем заказ-корзину
                var shoppingCartOrder = _orderProcessor.GetShoppingCartOrder(adv.ClientLegalPersonId);

                // Если заказа-корзины не существует - создаём её
                if (shoppingCartOrder == null)
                    shoppingCartOrder = _orderProcessor.CreateShoppingCartOrder(adv, dbTran);

                // Получааем скидку клиента
                var clientDiscount = _clientProcessor.GetDiscount(adv.ClientLegalPersonId, adv.Format.FormatTypeId);

                // Сохраняем позицию заказа
                int orderPositionId = _orderPositionProcessor.CreateFullOrderPosition(shoppingCartOrder.Id, null, clientDiscount, adv, dbTran);

                // Сохраняем пакетные позиции заказа
                foreach (var child in adv.Childs)
                    _orderPositionProcessor.CreateFullOrderPosition(shoppingCartOrder.Id, orderPositionId, clientDiscount, child, dbTran);

                // Обновляем заказ-корзину
                _orderProcessor.UpdateOrder(shoppingCartOrder.Id, dbTran);

                _context.SaveChanges();
                dbTran.Commit();
            }
        }

        #endregion

        #region Update
        public void UpdateAdvertisement(Advertisement adv)
        {
            using (var сontextTransaction = _context.Database.BeginTransaction())
            {
                var dbTran = сontextTransaction.GetDbTransaction();

                var orderPosition = _orderPositionProcessor.GetOrderPositionsByIds(new[] { adv.OrderPositionId }).Single();

                var shoppingCartOrder = _orderProcessor.GetShoppingCartOrder(adv.ClientLegalPersonId);

                // Обновляем позицию заказа
                _orderPositionProcessor.UpdateFullOrderPosition(orderPosition, adv, dbTran);

                // Перебираем переданные пакетные позиции
                if (adv.Childs != null)
                {
                    foreach (var child in adv.Childs)
                    {
                        var childOrderPosition = _orderPositionProcessor.GetOrderPositionsByIds(new[] { child.OrderPositionId }).SingleOrDefault();

                        // Если переданной позиции не существует - создаём её, иначе - обновляем
                        if (_orderPositionProcessor.NeedCreateFullOrderPosition(childOrderPosition))
                            _orderPositionProcessor.CreateFullOrderPosition(shoppingCartOrder.Id, orderPosition.Id, 0, child, dbTran);
                        else
                            _orderPositionProcessor.UpdateFullOrderPosition(childOrderPosition, child, dbTran);
                    }
                }

                var childOrderPositions = orderPosition.ChildOrderPositions;

                // Если в существующих пакетных позициях нет переданных - удаляем их
                foreach (var childOrderPosition in childOrderPositions)
                    if (_orderPositionProcessor.NeedDeleteFullOrderPosition(childOrderPosition.Id, adv.Childs))
                        _orderPositionProcessor.DeleteFullOrderPosition(childOrderPosition, dbTran);

                // Обновляем заказ-корзину
                _orderProcessor.UpdateOrder(shoppingCartOrder.Id, dbTran);

                _context.SaveChanges();
                dbTran.Commit();
            }
        }

        #endregion

        #region Delete
        public void DeleteAdvertisement(int orderPositionId)
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
