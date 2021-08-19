using CkpDAL.Entities;
using CkpModel.Input;
using CkpServices.Helpers.Factories.Interfaces;
using System;

namespace CkpServices.Helpers.Factories
{
    class OrderPositionFactory : IOrderPositionFactory
    {
        public OrderPosition Create(int orderId, int? parentOrderPositionId, float discount, float markup, float nds, OrderPositionData opd)
        {
            var orderPosition = new OrderPosition
            {
                Id = 0,
                ParentOrderPositionId = parentOrderPositionId,
                OrderId = orderId,
                SupplierId = opd.SupplierId,
                PriceId = opd.PriceId,
                PricePositionId = opd.FormatData.Id,
                PricePositionVersion = opd.FormatData.Version,
                Discount = discount,
                Markup = markup,
                Nds = nds,
                Compensation = 0,
                Description = string.Empty,
                ClientPackageDiscount = 0,
                BeginDate = DateTime.Now
            };

            return orderPosition;
        }
    }
}
