using CkpDAL.Model;
using CkpServices.Helpers.Factories.Interfaces.ClientAccount;
using System;
using System.Linq;

namespace CkpServices.Helpers.Factories.ClientAccount
{
    class AccountPositionFactory : IAccountPositionFactory
    {
        public AccountPosition Create(int accountId, OrderPosition orderPosition)
        {
            var count = GetPositionsCount(orderPosition);
            var sum = orderPosition.GetClientSum();
            var cost = sum / count;

            var accountPosition = new AccountPosition
            {
                Id = 0,
                AccountId = accountId,
                Nomenclature = GetAccountNumenclature(orderPosition),
                Name = GetAccountPositionName(orderPosition),
                Cost = cost,
                Count = count,
                Sum = sum,
                Discount = 0,
                FirstOutDate = GetFirstOutDate(orderPosition),
                BeginDate = DateTime.Now
            };

            return accountPosition;
        }

        private string GetAccountNumenclature(OrderPosition orderPosition)
        {
            string nomenclature;

            if (orderPosition.PricePosition.IsShowSize)
            {
                nomenclature = orderPosition.PricePosition.PricePositionType.EnableSecondSize
                    ? string.Format("{0} - {1} {2} ({3}x{4} {5}) {6}",
                        orderPosition.Supplier.Company.Name,
                        orderPosition.Supplier.City.Name,
                        orderPosition.PricePosition.Name,
                        orderPosition.PricePosition.FirstSize,
                        orderPosition.PricePosition.SecondSize,
                        orderPosition.PricePosition.Unit.Name,
                        orderPosition.PricePosition.Description ?? string.Empty)
                    : string.Format("{0} - {1} {2} ({3} {4}) {5}",
                        orderPosition.Supplier.Company.Name,
                        orderPosition.Supplier.City.Name,
                        orderPosition.PricePosition.Name,
                        orderPosition.PricePosition.FirstSize,
                        orderPosition.PricePosition.Unit.Name,
                        orderPosition.PricePosition.Description ?? string.Empty);
            }
            else
            {
                nomenclature = string.Format("{0} - {1} {2} {3}",
                    orderPosition.Supplier.Company.Name,
                    orderPosition.Supplier.City.Name,
                    orderPosition.PricePosition.Name,
                    orderPosition.PricePosition.Description ?? string.Empty);
            }

            return nomenclature;
        }

        private string GetAccountPositionName(OrderPosition orderPosition)
        {
            var pricePositionName = orderPosition.PricePosition.PricePositionType.EnableSecondSize
            ? string.Format("{0} ({1}x{2} {3})",
                orderPosition.PricePosition.Name,
                orderPosition.PricePosition.FirstSize,
                orderPosition.PricePosition.SecondSize,
                orderPosition.PricePosition.Unit.Name)
            : string.Format("{0} ({1} {2})",
                orderPosition.PricePosition.Name,
                orderPosition.PricePosition.FirstSize,
                orderPosition.PricePosition.Unit.Name);

            var outs = string.Join("; ", orderPosition.GraphicPositions
                .Select(gp => string.Format("{0}({1})", gp.Graphic.Number, gp.Graphic.OutDate.ToString("dd.MM.yyyy"))));

            var formulation = string.Format("Размещение объявлений о наборе персонала в издании \"{0}\" г. {1} формат {2} {3} №{4}",
                orderPosition.Supplier.FullName,
                orderPosition.Supplier.City.Name,
                orderPosition.PricePosition.PricePositionType.Name,
                pricePositionName,
                outs);

            return formulation;
        }

        private int GetPositionsCount(OrderPosition orderPosition)
        {
            var count = orderPosition.GraphicPositions
                .Where(gp => gp.Id == gp.ParenGraphicPositiontId)
                .Sum(gp => gp.Count);

            return count;
        }

        private DateTime GetFirstOutDate(OrderPosition orderPosition)
        {
            var firstOutDate = orderPosition.GraphicPositions
                .Select(gp => gp.Graphic.OutDate)
                .Min();

            return firstOutDate;
        }
    }
}
