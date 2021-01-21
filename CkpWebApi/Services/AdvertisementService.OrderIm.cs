using CkpWebApi.DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;

namespace CkpWebApi.Services
{
    public partial class AdvertisementService
    {
        private OrderIm GetOrderIm(int orderId, int orderImTypeId)
        {
            var orderIm = _context.OrderIms
                .SingleOrDefault(
                    oim =>
                        oim.OrderId == orderId &&
                        oim.OrderImType.Id == orderImTypeId);

            return orderIm;
        }

        private DateTime GetOrderImMaxClosingDate(int orderId, int orderImTypeId)
        {
            var orderImMaxClosingDate = _context.OrderPositions
                .Include(op => op.PricePosition.PricePositionType.PositionImType)
                .Include(op => op.GraphicPositions)
                .Where(
                    op =>
                        op.OrderId == orderId &&
                        op.PricePosition.PricePositionType.PositionImType.OrderImTypeId == orderImTypeId)
                .Select(op => op.GraphicPositions.Max(gp => gp.Graphic.ClosingDate))
                .AsEnumerable()
                .Max();

            return orderImMaxClosingDate;
        }

        private void SetOrderImStatusVerstka(int orderId, int orderImTypeId, DbTransaction dbTran)
        {
            var orderIm = GetOrderIm(orderId, orderImTypeId);
            orderIm.MaketStatusId = 3;
            UpdateOrderIm(orderIm, dbTran);
        }

        #region Create

        private void CreateOrderIm(int orderId, int orderImTypeId, DbTransaction dbTran)
        {
            //var maxClosingDate = GetOrderImMaxClosingDate(orderId, orderImTypeId);
            var lastEditDate = DateTime.Now;

            _repository.SetOrderIm(
                dbTran: dbTran,
                orderId: orderId,
                orderImTypeId: orderImTypeId,
                maketStatusId: 1, // 1 - Черновик, 3 - Вёрстка
                maketCategoryId: 2,
                replaceStatusId: 0,
                brief: string.Empty,
                needVisa: false,
                needVerify: false,
                designerId: null,
                comments: null,
                maxClosingDate: null,
                isViewed: false,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            SetNeedRefreshOrderShoppingCartOrder();
        }

        #endregion

        #region Update

        private bool NeedUpdateOrderIm(OrderIm orderIm)
        {
            var oldMaxClosingDate = orderIm.MaxClosingDate;
            var newMaxClosingDate = GetOrderImMaxClosingDate(orderIm.OrderId, orderIm.OrderImTypeId);

            return oldMaxClosingDate != newMaxClosingDate;

        }

        private void UpdateOrderIm(OrderIm orderIm, DbTransaction dbTran)
        {
            //var maxClosingDate = GetOrderImMaxClosingDate(orderIm.OrderId, orderIm.OrderImTypeId);
            var lastEditDate = orderIm.BeginDate;

            _repository.SetOrderIm(
                dbTran: dbTran,
                orderId: orderIm.OrderId,
                orderImTypeId: orderIm.OrderImTypeId,
                maketStatusId: orderIm.MaketStatusId,
                maketCategoryId: orderIm.MaketCategoryId,
                replaceStatusId: orderIm.ImReplaceStatusId,
                brief: orderIm.Brief,
                needVisa: orderIm.NeedVisa,
                needVerify: orderIm.NeedVerify,
                designerId: orderIm.DesignerId,
                comments: orderIm.Comments,
                maxClosingDate: orderIm.MaxClosingDate,
                isViewed: orderIm.IsViewed,
                isActual: true,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            _context.Entry(orderIm).Reload();
            SetNeedRefreshOrderShoppingCartOrder();
        }

        #endregion

        #region Delete

        private bool NeedDeleteOrderIm(OrderIm orderIm)
        {
            return !_context.PositionIms
                .Any(
                    pi =>
                        pi.OrderId == orderIm.OrderId &&
                        pi.PositionImType.OrderImTypeId == orderIm.OrderImTypeId);
        }

        private void DeleteOrderIm(OrderIm orderIm, DbTransaction dbTran)
        {
            var lastEditDate = orderIm.BeginDate;

            _repository.SetOrderIm(
                dbTran: dbTran,
                orderId: orderIm.OrderId,
                orderImTypeId: orderIm.OrderImTypeId,
                maketStatusId: orderIm.MaketStatusId,
                maketCategoryId: orderIm.MaketCategoryId,
                replaceStatusId: orderIm.MaketStatusId,
                brief: orderIm.Brief,
                needVisa: orderIm.NeedVisa,
                needVerify: orderIm.NeedVerify,
                designerId: orderIm.DesignerId,
                comments: orderIm.Comments,
                maxClosingDate: orderIm.MaxClosingDate,
                isViewed: orderIm.IsViewed,
                isActual: false,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            _context.Entry(orderIm).Reload();
            SetNeedRefreshOrderShoppingCartOrder();
        }

        #endregion
    }
}
