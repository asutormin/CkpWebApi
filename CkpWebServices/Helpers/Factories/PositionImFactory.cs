using CkpDAL.Entities;
using CkpServices.Helpers.Factories.Interfaces;
using System;

namespace CkpServices.Helpers.Factories
{
    class PositionImFactory : IPositionImFactory
    {
        public PositionIm Create(int orderId, int orderPositionId, int positionImTypeId)
        {
            var positionIm = new PositionIm
            {
                OrderId = orderId,
                OrderPositionId = orderPositionId,
                PositionImTypeId = positionImTypeId,
                ParentPositionId = null,
                MaketStatusId = 1, // 1 - Черновик, 4 - Готово
                MaketCategoryId = 2,
                Text = string.Empty,
                Comments = string.Empty,
                Url = string.Empty,
                DistributionUrl = string.Empty,
                LegalPersonPersonalDataId = null,
                Xml = null,
                Rating = 0,
                RatingDescription = string.Empty,
                TaskFileDate = null,
                MaketFileDate = null,
                DontVerify = false,
                RdvRating = false,
                BeginDate = DateTime.Now
            };

            return positionIm;
        }
    }
}
