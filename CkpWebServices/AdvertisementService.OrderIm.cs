using CkpDAL.Model;
using CkpServices.Helpers.Factories;
using CkpServices.Helpers.Factories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;

namespace CkpServices
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

        private void ProcessOrderImStatus(OrderIm orderIm, DbTransaction dbTran,
            Action<OrderIm, bool, DbTransaction> action)
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

            if (action != null)
                action(orderIm, true, dbTran);
        }

        #region Set

        private void SetOrderIm(OrderIm orderIm, bool isActual, DbTransaction dbTran)
        {
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
                isActual: isActual,
                lastEditDate: ref lastEditDate,
                editUserId: _editUserId);

            _context.Entry(orderIm).Reload();
        }

        #endregion

        #region Create

        private void CreateOrderIm(int orderId, int orderImTypeId, DbTransaction dbTran)
        {
            var orderIm = _orderImFactory.Create(orderId, orderImTypeId);

            SetOrderIm(orderIm, true, dbTran);
        }

        #endregion

        #region Update

        private bool NeedUpdateOrderIm(OrderIm orderIm)
        {
            var oldMaxClosingDate = orderIm.MaxClosingDate;
            var newMaxClosingDate = GetOrderImMaxClosingDate(orderIm.OrderId, orderIm.OrderImTypeId);

            return oldMaxClosingDate != newMaxClosingDate;

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
            SetOrderIm(orderIm, false, dbTran);
        }

        #endregion
    }
}
