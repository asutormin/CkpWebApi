using CkpDAL;
using CkpDAL.Model;
using CkpDAL.Repository;
using CkpEntities.Input;
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
    class OrderPositionProcessor : IOrderPositionProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;
        
        private readonly int _orderBusinessUnitId;

        private readonly IRubricProcessor _rubricProcessor;
        private readonly IGraphicProcessor _graphicProcessor;
        private readonly IPositionImProcessor _positionImProcessor;

        private readonly IOrderPositionFactory _orderPositionFactory;

        public OrderPositionProcessor(
            BPFinanceContext context,
            IBPFinanceRepository repository,
            IRubricProcessor rubricProcessor,
            IGraphicProcessor graphicProcessor,
            IPositionImProcessor positionImProcessor,
            int orderBusinessUnitId)
        {
            _context = context;
            _repository = repository;

            _orderBusinessUnitId = orderBusinessUnitId;

            _rubricProcessor = rubricProcessor;
            _graphicProcessor = graphicProcessor;
            _positionImProcessor = positionImProcessor;

            _orderPositionFactory = new OrderPositionFactory();
        }

        #region Get

        public IEnumerable<OrderPosition> GetOrderPositionsByIds(int[] orderPositionIds)
        {
            var orderPositions = _context.OrderPositions
                .Include(op => op.Order).ThenInclude(o => o.OrderPositions).ThenInclude(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.Supplier).ThenInclude(su => su.Company)
                .Include(op => op.Supplier).ThenInclude(su => su.City)
                .Include(op => op.Price)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.PricePositionType)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.Unit)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.RubricPositions)
                .Include(op => op.PositionIm).ThenInclude(pi => pi.PositionImType).ThenInclude(pit => pit.OrderImType)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Price)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.RubricPositions)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.GraphicPositions).ThenInclude(cgp => cgp.Graphic)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PositionIm).ThenInclude(cpi => cpi.PositionImType).ThenInclude(pit => pit.OrderImType)
                .Where(op => orderPositionIds.Contains(op.Id))
                .ToList();

            return orderPositions;
        }

        #endregion

        #region Create

        public int CreateFullOrderPosition(int orderId, int? parentOrderPositionId, float clientDiscount, Advertisement adv,
            DbTransaction dbTran)
        {
            // Создаём позицию заказа
            var orderPosition = CreateOrderPosition(orderId, parentOrderPositionId, clientDiscount, adv,
                dbTran);

            // Если рубрика не задана (пакет) - не сохраняем её
            if (adv.Rubric != null)
                _rubricProcessor.CreateRubricPosition(orderPosition.Id, adv.Rubric, dbTran);

            // Если графики не переданы - ищем график на основе пакетных позиций
            if (!adv.Graphics.Any())
                adv.Graphics = _graphicProcessor.GetPackageAdvGraphics(adv);

            _graphicProcessor.CreateGraphicPositions(orderPosition.Id, adv.Graphics, dbTran);

            _positionImProcessor.CreatePositionIm(_orderBusinessUnitId, orderId, orderPosition.Id, adv, dbTran);

            return orderPosition.Id;
        }

        private OrderPosition CreateOrderPosition(int orderId, int? parentOrderPositionId, float clientDiscount, Advertisement adv,
            DbTransaction dbTran)
        {
            var markup = _context.Prices.Single(pr => pr.Id == adv.PriceId).Markup;
            var nds = GetNds(_orderBusinessUnitId, adv.SupplierId);

            var orderPosition = _orderPositionFactory.Create(orderId, parentOrderPositionId, clientDiscount, markup, nds, adv);
            orderPosition = _repository.SetOrderPosition(orderPosition, false, isActual: true, dbTran);

            return orderPosition;
        }

        public bool NeedCreateFullOrderPosition(OrderPosition orderPosition)
        {
            return orderPosition == null;
        }

        #endregion

        #region Update

        public void UpdateFullOrderPosition(OrderPosition orderPosition, Advertisement adv, DbTransaction dbTran)
        {
            UpdateOrderPosition(orderPosition, adv, dbTran);

            // Если рубрика не задана - удаляем все позиции рубрик
            if (adv.Rubric == null || adv.Rubric.Id == 0)
                _rubricProcessor.DeleteRubricPositions(orderPosition.RubricPositions, dbTran);
            else
                _rubricProcessor.UpdateRubricPosition(orderPosition.Id, orderPosition.RubricPositions, adv.Rubric, dbTran);

            // Если графики не переданы - ищем график на основе пакетных позиций
            if (adv.Graphics == null)
                adv.Graphics = _graphicProcessor.GetPackageAdvGraphics(adv);

            _graphicProcessor.UpdateGraphicPositions(orderPosition.Id, orderPosition.GraphicPositions, adv.Graphics, dbTran);

            _positionImProcessor.UpdatePositionIm(orderPosition.PositionIm, adv, dbTran);
        }

        public void UpdateOrderPosition(OrderPosition orderPosition, Advertisement adv, DbTransaction dbTran)
        {
            if (!NeedUpdateOrderPosition(orderPosition, adv))
                return;

            orderPosition.Id = adv.OrderPositionId;
            orderPosition.SupplierId = adv.SupplierId;
            orderPosition.PriceId = adv.PriceId;
            orderPosition.PricePositionId = adv.Format.Id;
            orderPosition.PricePositionVersion = adv.Format.Version;
            orderPosition.BeginDate = DateTime.Now;

            var isUnloaded = IsUnloaded(orderPosition);

            _repository.SetOrderPosition(orderPosition, isUnloaded, isActual: true, dbTran);
        }

        public void UpdateOrderPosition(OrderPosition orderPosition, int orderId, DbTransaction dbTran)
        {
            if (orderPosition.OrderId == orderId)
                return;

            orderPosition.OrderId = orderId;

            var isUnloaded = IsUnloaded(orderPosition); 

            _repository.SetOrderPosition(orderPosition, isUnloaded, isActual: true, dbTran);
        }

        private bool NeedUpdateOrderPosition(OrderPosition orderPosition, Advertisement adv)
        {
            if (orderPosition.Id == adv.OrderPositionId &&
                orderPosition.SupplierId == adv.SupplierId &&
                orderPosition.PricePositionId == adv.Format.Id &&
                orderPosition.PricePositionVersion == adv.Format.Version &&
                orderPosition.PriceId == adv.PriceId)
                            return false;

            return true;
        }

        #endregion

        #region Delete

        public void DeleteFullOrderPosition(OrderPosition orderPosition, DbTransaction dbTran)
        {
            _positionImProcessor.DeletePositionIm(orderPosition.PositionIm, dbTran);

            _rubricProcessor.DeleteRubricPositions(orderPosition.RubricPositions, dbTran);
            _graphicProcessor.DeleteGraphicPositions(orderPosition.GraphicPositions, dbTran);

            var isUnloaded = IsUnloaded(orderPosition);
            _repository.SetOrderPosition(orderPosition, isUnloaded, isActual: false, dbTran);
            _context.Entry(orderPosition).Reload();
        }
        public bool NeedDeleteFullOrderPosition(int orderPositionId, List<Advertisement> advs)
        {
            return !advs.Any(adv => adv.OrderPositionId == orderPositionId);
        }

        #endregion

        private float GetNds(int businessUnitId, int supplierId)
        {
            // Получаем НДС на основании бизнес юнита и поставщика
            var supplier = _context.Suppliers.Single(su => su.Id == supplierId);
            var businessUnit = _context.BusinessUnits.Single(bu => bu.Id == businessUnitId);

            float nds = 0;
            if (businessUnit.AccountsWithNds)
            {
                if (supplier.IsNeedNds == 2)
                    float.TryParse(supplier.Nds, out nds);
            }

            return nds;
        }

        private bool IsUnloaded(OrderPosition orderPosition)
        {
            return orderPosition.GraphicPositions.Any(gp => gp.UnloadingPosition != null);
        }
    }
}
