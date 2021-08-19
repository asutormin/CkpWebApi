using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpModel.Input;
using CkpServices.Helpers.Converters;
using CkpServices.Helpers.Factories;
using CkpServices.Helpers.Factories.Interfaces;
using CkpServices.Processors.Interfaces;
using System;
using System.Data.Common;

namespace CkpServices.Processors
{
    class PositionImProcessor : IPositionImProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IOrderImProcessor _orderImProcessor;
        private readonly IStringProcessor _stringProcessor;
        private readonly IModuleProcessor _moduleProcessor;
        private readonly IPositionImFactory _positionImFactory;

        public PositionImProcessor(
            BPFinanceContext context,
            IBPFinanceRepository repository,
            IOrderImProcessor orderImProcessor,
            IStringProcessor stringProcessor,
            IModuleProcessor moduleProcessor)
        {
            _context = context;
            _repository = repository;

            _orderImProcessor = orderImProcessor;
            _stringProcessor = stringProcessor;
            _moduleProcessor = moduleProcessor;

            _positionImFactory = new PositionImFactory();

        }

        #region Create

        public void CreatePositionIm(int businessUnitId, int orderId, int orderPositionId, OrderPositionData opd, DbTransaction dbTran)
        {
            Action<int, int, DbTransaction> setOrderIm = (orderId, orderImTypeId, dbTran) =>
            {
                var orderIm = _orderImProcessor.GetOrderIm(orderId, orderImTypeId);

                if (orderIm == null)
                    _orderImProcessor.CreateOrderIm(orderId, orderImTypeId, null, dbTran);
            };

            if (opd.StringData != null)
            {
                var positionIm = _positionImFactory.Create(orderId, orderPositionId, positionImTypeId: 1);
                _repository.SetPositionIm(positionIm, newTaskFile: false, newMaketFile: false, isActual: true, dbTran);

                _stringProcessor.CreateFullString(businessUnitId, opd.ClientId, orderPositionId, opd.StringData, dbTran);

                setOrderIm(orderId, 1, dbTran);
            }

            if (opd.ModuleData != null)
            {
                var positionIm = _positionImFactory.Create(orderId, orderPositionId, positionImTypeId: 2);
                positionIm = _repository.SetPositionIm(positionIm, newTaskFile: true, newMaketFile: false, isActual: true, dbTran);

                var bytes = Base64ToBytesConverter.Convert(opd.ModuleData.Base64String);
                var taskFileDate = (DateTime)positionIm.TaskFileDate;

                _moduleProcessor.CreateSampleImage(orderPositionId, bytes, "ImgTask", taskFileDate);
                _moduleProcessor.CreateModuleGraphics(orderPositionId, bytes, opd.ModuleData.Name);

                setOrderIm(orderId, 2, dbTran);
            }
        }

        #endregion

        #region Update

        public void UpdatePositionIm(PositionIm positionIm, OrderPositionData opd, DbTransaction dbTran)
        {
            if (NeedReCreatePositionIm(positionIm, opd.FormatData.Id))
            {
                DeletePositionIm(positionIm, dbTran);
                CreatePositionIm(positionIm.OrderPosition.Order.BusinessUnitId, opd.OrderId, opd.OrderPositionId, opd, dbTran);
                return;
            }

            if (_stringProcessor.CanUpdateString(positionIm))
            {
                positionIm = _repository.SetPositionIm(positionIm, newTaskFile: false, newMaketFile: false, isActual: true, dbTran);
                _stringProcessor.UpdateFullString(positionIm.OrderPositionId, opd.StringData, dbTran);
            }

            if (_moduleProcessor.CanUpdateModule(positionIm))
            {
                positionIm = _repository.SetPositionIm(positionIm, newTaskFile: true, newMaketFile: false, isActual: true, dbTran);
                
                // Обновляем графические материалы позиции ИМ-а
                var taskFileDate = (DateTime)positionIm.TaskFileDate;
                var bytes = Base64ToBytesConverter.Convert(opd.ModuleData.Base64String);

                _moduleProcessor.CreateSampleImage(positionIm.OrderPositionId, bytes, "ImgTask", taskFileDate);
                _moduleProcessor.UpdateModuleGraphics(positionIm.OrderPositionId, bytes, opd.ModuleData.Name);
            }
        }

        public PositionIm UpdatePositionIm(PositionIm positionIm, int orderId, int maketStatusId, DbTransaction dbTran)
        {
            positionIm.OrderId = orderId; // Меняем заказ позиции ИМ-а
            positionIm.MaketStatusId = maketStatusId; // Меняем статус позиции ИМ-а на "Вёрстка"
            positionIm = _repository.SetPositionIm(positionIm, newTaskFile: false, newMaketFile: false, isActual: true, dbTran);

            return positionIm;
        }

        private bool NeedReCreatePositionIm(PositionIm positionIm, int pricePositionId)
        {
            return positionIm != null && positionIm.OrderPosition.PricePositionId != pricePositionId;
        }

        #endregion

        #region Delete

        public void DeletePositionIm(PositionIm positionIm, DbTransaction dbTran)
        {
            if (positionIm == null)
                return;

            _stringProcessor.DeleteFullString(positionIm.OrderPositionId, dbTran);
            _moduleProcessor.DeleteModuleGraphics(positionIm.OrderPositionId);

            _repository.SetPositionIm(positionIm, newTaskFile: false, newMaketFile: false, isActual: false, dbTran);
            _context.Entry(positionIm).Reload();

            var orderIm = _orderImProcessor.GetOrderIm(positionIm.OrderId, positionIm.PositionImType.OrderImTypeId);

            if (_orderImProcessor.NeedDeleteOrderIm(orderIm))
                _orderImProcessor.DeleteOrderIm(orderIm, dbTran);
            else
                _orderImProcessor.UpdateOrderIm(orderIm, dbTran);
        }

        #endregion
    }
}
