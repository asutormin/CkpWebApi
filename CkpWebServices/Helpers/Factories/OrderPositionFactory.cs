using CkpDAL.Model;
using CkpEntities.Input;
using CkpServices.Helpers.Factories.Interfaces;
using System;

namespace CkpServices.Helpers.Factories
{
    class OrderPositionFactory : IOrderPositionFactory
    {
        public OrderPosition Create(int orderId, int? parentOrderPositionId, float discount, float markup, float nds, Advertisement adv)
        {
            var orderPosition = new OrderPosition
            {
                Id = 0,
                ParentOrderPositionId = parentOrderPositionId,
                OrderId = orderId,
                SupplierId = adv.SupplierId,
                PriceId = adv.PriceId,
                PricePositionId = adv.Format.Id,
                PricePositionVersion = adv.Format.Version,
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
