using CkpDAL.Model;
using CkpEntities.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices
{
    public partial class AdvertisementService
    {
        private bool IsOrderPositionUnloaded(OrderPosition orderPosition)
        {
            return orderPosition.GraphicPositions
                .Any(gp => gp.UnloadingPosition != null);
        }

        #region Create

        private bool NeedCreateFullOrderPosition(OrderPosition orderPosition)
        {
            return orderPosition == null;
        }

        private int CreateOrderPosition(int orderId, int? parentOrderPositionId, float clientDiscount, Advertisement adv, DbTransaction dbTran)
        {
            var orderPositionId = adv.OrderPositionId;

            var markup = _context.Prices.Single(pr => pr.Id == adv.PriceId).Markup;
            var nds = GetOrderPositionNds(_orderBusinessUnitId, adv.SupplierId);

            var lastEditDate = DateTime.Now;

            _repository.SetOrderPosition(
                dbTran: dbTran,
                id: ref orderPositionId,
                orderId: orderId,
                parentOrderPositionId: parentOrderPositionId,
                supplierId: adv.SupplierId,
                priceId: adv.PriceId,
                pricePositionId: adv.Format.Id,
                pricePositionVersionDate: adv.Format.Version,
                clientDiscount: clientDiscount,
                markup: markup,
                nds: nds,
                compensation: 0,
                description: string.Empty,
                clientPackageDiscount: 0,
                editUserId: _editUserId,
                isActual: true,
                lastEditDate: ref lastEditDate,
                isUnloaded: false,
                needConfirmation: false);

            return orderPositionId;
        }

        private int CreateFullOrderPosition(int orderId, int? parentOrderPositionId, float clientDiscount, Advertisement adv,
            DbTransaction dbTran)
        {
            var orderPositionId = CreateOrderPosition(orderId, parentOrderPositionId, clientDiscount, adv, dbTran);
            
            // Если рубрика не задана (пакет) - не сохраняем её
            if (adv.Rubric != null)
                CreateRubricPosition(orderPositionId, adv.Rubric, dbTran);

            // Если графики не переданы - ищем график на основе пакетных позиций
            if (!adv.Graphics.Any())
                adv.Graphics = GetPackageAdvGraphics(adv);

            CreateGraphicPositions(orderPositionId, adv.Graphics, dbTran);

            CreateFullPositionIm(orderId, orderPositionId, adv, dbTran);

            return orderPositionId;
        }

        private float GetOrderPositionNds(int businessUnitId, int supplierId)
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

        private float GetClientDiscount(int legalPersonId, int pricePositionTypeId)
        {
            var pricePositionTypeDiscount = _context.LegalPersons
                .Include(lp => lp.PricePositionTypeDiscounts)
                .Single(lp => lp.Id == legalPersonId)
                .PricePositionTypeDiscounts
                .SingleOrDefault(pptd => pptd.PricePositionTypeId == pricePositionTypeId);

            return pricePositionTypeDiscount == null 
                ? 0
                : pricePositionTypeDiscount.DiscountPercent;
        }

        #endregion

        #region Update

        private bool NeedUpdateOrderPosition(OrderPosition orderPosition, Advertisement adv)
        {
            return
                orderPosition.Id != adv.OrderPositionId ||
                orderPosition.SupplierId != adv.SupplierId ||
                orderPosition.PricePositionId != adv.Format.Id ||
                orderPosition.PricePositionVersion != adv.Format.Version ||
                orderPosition.PriceId != adv.PriceId;
        }

        private void UpdateOrderPosition(OrderPosition orderPosition, Advertisement adv, DbTransaction dbTran)
        {
            var orderPositionId = adv.OrderPositionId;

            if (!NeedUpdateOrderPosition(orderPosition, adv))
                return;

            var parentOrderPositionId =
                orderPosition.ParentOrderPositionId == null
                ? 0
                : (int)orderPosition.ParentOrderPositionId;

            var isUnloaded = IsOrderPositionUnloaded(orderPosition);

            var lastEditDate = orderPosition.BeginDate;

            _repository.SetOrderPosition(
                dbTran: dbTran,
                id: ref orderPositionId,
                orderId: orderPosition.OrderId,
                parentOrderPositionId: parentOrderPositionId,
                supplierId: adv.SupplierId,
                priceId: adv.PriceId,
                pricePositionId: adv.Format.Id,
                pricePositionVersionDate: adv.Format.Version,
                clientDiscount: orderPosition.Discount,
                markup: orderPosition.Markup,
                nds: orderPosition.Nds,
                compensation: orderPosition.Compensation, //
                description: orderPosition.Description,
                clientPackageDiscount: orderPosition.ClientPackageDiscount,
                editUserId: _editUserId,
                isActual: true,
                lastEditDate: ref lastEditDate,
                isUnloaded: isUnloaded,
                needConfirmation: false);

            _context.Entry(orderPosition).Reload();
        }

        private void UpdateOrderPosition(OrderPosition orderPosition, DbTransaction dbTran)
        {
            var orderPositionId = orderPosition.Id;

            var isUnloaded = IsOrderPositionUnloaded(orderPosition);
            var lastEditDate = orderPosition.BeginDate;

            _repository.SetOrderPosition(
                dbTran: dbTran,
                id: ref orderPositionId,
                orderId: orderPosition.OrderId,
                parentOrderPositionId: orderPosition.ParentOrderPositionId,
                supplierId: orderPosition.SupplierId,
                priceId: orderPosition.PriceId,
                pricePositionId: orderPosition.PricePositionId,
                pricePositionVersionDate: orderPosition.PricePositionVersion,
                clientDiscount: orderPosition.Discount,
                markup: orderPosition.Markup,
                nds: orderPosition.Nds,
                compensation: orderPosition.Compensation, //
                description: orderPosition.Description,
                clientPackageDiscount: orderPosition.ClientPackageDiscount,
                editUserId: _editUserId,
                isActual: true,
                lastEditDate: ref lastEditDate,
                isUnloaded: isUnloaded,
                needConfirmation: false);

            _context.Entry(orderPosition).Reload();
        }

        private void UpdateFullOrderPosition(OrderPosition orderPosition, Advertisement adv, DbTransaction dbTran)
        {
            UpdateOrderPosition(orderPosition, adv, dbTran);

            // Если рубрика не задана - удаляем все позиции рубрик
            if (adv.Rubric != null)
                DeleteRubricPositions(orderPosition.RubricPositions, dbTran);
            else
                UpdateRubricPosition(orderPosition.Id, orderPosition.RubricPositions, adv.Rubric, dbTran);

            // Если графики не переданы - ищем график на основе пакетных позиций
            if (adv.Graphics == null)
                adv.Graphics = GetPackageAdvGraphics(adv);

            UpdateGraphicPositions(orderPosition.Id, orderPosition.GraphicPositions, adv.Graphics, dbTran);

            UpdateFullPositionIm(orderPosition.PositionIm, adv, dbTran);
        }

        #endregion

        #region Delete

        public bool NeedDeleteFullOrderPosition(int orderPositionId, List<Advertisement> advs)
        {
            return !advs.Any(adv => adv.OrderPositionId == orderPositionId);
        }

        private void DeleteOrderPosition(OrderPosition orderPosition, DbTransaction dbTran)
        {
            var orderPositionId = orderPosition.Id;

            var parentOrderPositionId =
                orderPosition.ParentOrderPositionId == null
                    ? 0
                    : (int)orderPosition.ParentOrderPositionId;

            var isUnloaded = orderPosition.GraphicPositions
                .Any(gp => gp.UnloadingPosition != null);

            var lastEditDate = orderPosition.BeginDate;

            _repository.SetOrderPosition(
                dbTran: dbTran,
                id: ref orderPositionId,
                orderId: orderPosition.OrderId,
                parentOrderPositionId: parentOrderPositionId,
                supplierId: orderPosition.SupplierId,
                priceId: orderPosition.PriceId,
                pricePositionId: orderPosition.PricePositionId,
                pricePositionVersionDate: orderPosition.PricePositionVersion,
                clientDiscount: orderPosition.Discount,
                markup: orderPosition.Markup,
                nds: orderPosition.Nds,
                compensation: orderPosition.Compensation,
                description: orderPosition.Description,
                clientPackageDiscount: orderPosition.ClientPackageDiscount,
                editUserId: _editUserId,
                isActual: false,
                lastEditDate: ref lastEditDate,
                isUnloaded: isUnloaded,
                needConfirmation: orderPosition.NeedConfirmation);

            _context.Entry(orderPosition).Reload();
        }

        private void DeleteFullOrderPosition(OrderPosition orderPosition, DbTransaction dbTran)
        {
            DeleteFullPositionIm(orderPosition.PositionIm, dbTran);

            DeleteRubricPositions(orderPosition.RubricPositions, dbTran);
            DeleteGraphicPositions(orderPosition.GraphicPositions, dbTran);
            
            DeleteOrderPosition(orderPosition, dbTran);
        }

        #endregion
    }
}
