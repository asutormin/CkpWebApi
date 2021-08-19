using CkpDAL.Entities;
using CkpServices.Helpers.Factories.Interfaces;
using System;

namespace CkpServices.Helpers.Factories
{
    class OrderImFactory : IOrderImFactory
    {
        public OrderIm Create(int orderId, int orderImTypeId)
        {
            var orderIm = new OrderIm
            {
                OrderId = orderId,
                OrderImTypeId = orderImTypeId,
                MaketStatusId = 1, // 1 - Черновик, 3 - Вёрстка
                MaketCategoryId = 2,
                ImReplaceStatusId = 0,
                Brief = string.Empty,
                NeedVisa = false,
                NeedVerify = false,
                DesignerId = null,
                Comments = null,
                MaxClosingDate = null,
                IsViewed = false,
                BeginDate = DateTime.Now
            };

            return orderIm;
        }
    }
}
