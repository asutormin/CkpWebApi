using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpInfrastructure.Providers.Interfaces;
using CkpModel.Input;
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
         
        private readonly IRubricProcessor _rubricProcessor;
        private readonly IGraphicProcessor _graphicProcessor;
        private readonly IPositionImProcessor _positionImProcessor;

        private readonly string _basketOrderDescription;

        private readonly IOrderPositionFactory _orderPositionFactory;

        public OrderPositionProcessor(
            BPFinanceContext context,
            IBPFinanceRepository repository,
            IRubricProcessor rubricProcessor,
            IGraphicProcessor graphicProcessor,
            IPositionImProcessor positionImProcessor,
            string basketOrderDescription)
        {
            _context = context;
            _repository = repository;

            _rubricProcessor = rubricProcessor;
            _graphicProcessor = graphicProcessor;
            _positionImProcessor = positionImProcessor;

            _basketOrderDescription = basketOrderDescription;

            _orderPositionFactory = new OrderPositionFactory();
        }

        #region Get

        public IEnumerable<OrderPosition> GetInnerOperationsOrderPositionsByIds(int[] orderPositionIds)
        {
            var orderPositions = _context.OrderPositions
                .Include(op => op.Order).ThenInclude(o => o.OrderPositions).ThenInclude(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.Supplier).ThenInclude(su => su.Company)
                .Include(op => op.Supplier).ThenInclude(su => su.City)
                .Include(op => op.Price)
                .Include(op => op.PricePosition).ThenInclude(op => op.PricePositionEx)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.PricePositionType)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.Unit)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.RubricPositions)
                .Include(op => op.PositionIm).ThenInclude(pi => pi.PositionImType).ThenInclude(pit => pit.OrderImType)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Supplier).ThenInclude(csu => csu.Company)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Supplier).ThenInclude(csu => csu.City)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Price)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PricePosition).ThenInclude(cpp => cpp.PricePositionEx)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PricePosition).ThenInclude(cpp => cpp.PricePositionType)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PricePosition).ThenInclude(cpp => cpp.Unit)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.RubricPositions)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.GraphicPositions).ThenInclude(cgp => cgp.Graphic)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PositionIm).ThenInclude(cpi => cpi.PositionImType).ThenInclude(pit => pit.OrderImType)
                .Where(op => orderPositionIds.Contains(op.Id))
                .ToList();

            return orderPositions;
        }

        public IQueryable<OrderPosition> GetBasketOrderPositionsQuery(int clientLegalPersonId)
        {
            return GetOrderPositionsQuery(clientLegalPersonId, null, 0, 20, _basketOrderDescription);
        }

        public IQueryable<OrderPosition> GetAccountOrderPositionsQuery(int clientLegalPersonId, int accountId)
        {
            return GetOrderPositionsQuery(clientLegalPersonId, null, accountId, 0, null);
        }

        public IQueryable<OrderPosition> GetOrderPositionsByIdsOuery(int clientLegalPersonId, int[] orderPositionIds)
        {
            return GetOrderPositionsQuery(clientLegalPersonId, orderPositionIds, 0, 0, null);
        }

        private IQueryable<OrderPosition> GetOrderPositionsQuery(int clientLegalPersonId, int[] orderPositionIds, int accountId, int activityTypeId, string description)
        {
            var query = _context.OrderPositions
                .Include(op => op.Order.AccountOrder.Account)
                .Include(op => op.Supplier).ThenInclude(su => su.Company)
                .Include(op => op.Supplier).ThenInclude(su => su.City)
                .Include(op => op.Price)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.PricePositionType)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.RubricPositions).ThenInclude(rp => rp.Rubric)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Supplier).ThenInclude(csu => csu.Company)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Supplier).ThenInclude(csu => csu.City)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.Price)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.PricePosition).ThenInclude(cpp => cpp.PricePositionType)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.GraphicPositions).ThenInclude(cgp => cgp.Graphic)
                .Include(op => op.ChildOrderPositions).ThenInclude(cop => cop.RubricPositions).ThenInclude(crp => crp.Rubric)
                .Where(
                    op =>
                        op.ParentOrderPositionId == null &&
                        op.Order.ClientLegalPersonId == clientLegalPersonId &&
                        (orderPositionIds == null || orderPositionIds.Contains(op.Id)) &&
                        (accountId == 0 || op.Order.AccountOrder.AccountId == accountId) &&
                        (activityTypeId == 0 || op.Order.ActivityTypeId == activityTypeId) &&
                        (description == null || op.Order.Description == description));

            return query;
        }

        #endregion

        #region Create

        public int CreateFullOrderPosition(int businessUnitId, int orderId, int? parentOrderPositionId, float clientDiscount, OrderPositionData opd,
            DbTransaction dbTran)
        {
            // Создаём позицию заказа
            var orderPosition = CreateOrderPosition(orderId, parentOrderPositionId, clientDiscount, opd,
                dbTran);

            // Если рубрика не задана (пакет) - не сохраняем её
            if (opd.RubricData != null)
                _rubricProcessor.CreateRubricPosition(orderPosition.Id, opd.RubricData, dbTran);

            // Если передан пакет - ищем график на основе его позиций
            if (opd.Childs.Any())
                opd.GraphicsData = _graphicProcessor.GetPackageGraphicsData(opd);

            _graphicProcessor.CreateGraphicPositions(orderPosition.Id, opd.GraphicsData, dbTran);
             
            _positionImProcessor.CreatePositionIm(businessUnitId, orderId, orderPosition.Id, opd, dbTran);

            return orderPosition.Id;
        }

        private OrderPosition CreateOrderPosition(int orderId, int? parentOrderPositionId, float clientDiscount, OrderPositionData opd,
            DbTransaction dbTran)
        {
            var markup = _context.Prices.Single(pr => pr.Id == opd.PriceId).Markup;
            var businessUnitId = _context.Prices.Single(p => p.Id == opd.PriceId).BusinessUnitId;
            var nds = GetNds(businessUnitId, opd.SupplierId);

            var orderPosition = _orderPositionFactory.Create(orderId, parentOrderPositionId, clientDiscount, markup, nds, opd);
            orderPosition = _repository.SetOrderPosition(orderPosition, false, isActual: true, dbTran);

            return orderPosition;
        }

        public bool NeedCreateFullOrderPosition(OrderPosition orderPosition)
        {
            return orderPosition == null;
        }

        #endregion

        #region Update

        public void UpdateFullOrderPosition(OrderPosition orderPosition, OrderPositionData opd, DbTransaction dbTran)
        {
            UpdateOrderPosition(orderPosition, opd, dbTran);

            // Если рубрика не задана - удаляем все позиции рубрик
            if (opd.RubricData == null || opd.RubricData.Id == 0)
                _rubricProcessor.DeleteRubricPositions(orderPosition.RubricPositions, dbTran);
            else
                _rubricProcessor.UpdateRubricPosition(orderPosition.Id, orderPosition.RubricPositions, opd.RubricData, dbTran);

            // Если переданы есть пакетные позиции - вычисляем график на их основе
            if (opd.Childs.Any())
                opd.GraphicsData = _graphicProcessor.GetPackageGraphicsData(opd);

            _graphicProcessor.UpdateGraphicPositions(orderPosition.Id, orderPosition.GraphicPositions, opd.GraphicsData, dbTran);

            _positionImProcessor.UpdatePositionIm(orderPosition.PositionIm, opd, dbTran);
        }

        public void UpdateOrderPosition(OrderPosition orderPosition, OrderPositionData opd, DbTransaction dbTran)
        {
            if (!NeedUpdateOrderPosition(orderPosition, opd))
                return;

            orderPosition.Id = opd.OrderPositionId;
            orderPosition.SupplierId = opd.SupplierId;
            orderPosition.PriceId = opd.PriceId;
            orderPosition.PricePositionId = opd.FormatData.Id;
            orderPosition.PricePositionVersion = opd.FormatData.Version;
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

        private bool NeedUpdateOrderPosition(OrderPosition orderPosition, OrderPositionData opd)
        {
            if (orderPosition.Id == opd.OrderPositionId &&
                orderPosition.SupplierId == opd.SupplierId &&
                orderPosition.PricePositionId == opd.FormatData.Id &&
                orderPosition.PricePositionVersion == opd.FormatData.Version &&
                orderPosition.PriceId == opd.PriceId)
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
        public bool NeedDeleteFullOrderPosition(int orderPositionId, List<OrderPositionData> opds)
        {
            return !opds.Any(adv => adv.OrderPositionId == orderPositionId);
        }

        #endregion

        private float GetNds(int businessUnitId, int supplierId)
        {
            // Получаем бизнес юнит
            var businessUnit = _context.BusinessUnits
                .Single(bu => bu.Id == businessUnitId);

            float nds = 0;
            // Если бизнес юнит НДС-ный
            if (businessUnit.AccountsWithNds)
            {
                // Проверяем НДС-ность поставщика
                // Если поставщик без НДС-ный - получаем значение НДС
                var supplier = _context.Suppliers.Single(su => su.Id == supplierId);
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
