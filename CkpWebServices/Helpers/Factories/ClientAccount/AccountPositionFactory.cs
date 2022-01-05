using CkpDAL.Entities;
using CkpServices.Helpers.Factories.Interfaces.ClientAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CkpServices.Helpers.Factories.ClientAccount
{
    class AccountPositionFactory : IAccountPositionFactory
    {
        public AccountPosition Create(int accountId, OrderPosition orderPosition, List<OrderPosition> packagePositions)
        {
            var count = GetPositionsCount(orderPosition);
            var sum = orderPosition.GetClientSum();
            var cost = sum / count;

            var accountPosition = new AccountPosition
            {
                Id = 0,
                AccountId = accountId,
                Nomenclature = GetAccountNumenclature(orderPosition),
                Name = GetAccountPositionName(orderPosition, packagePositions),
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

        private string GetAccountPositionName(OrderPosition orderPosition, List<OrderPosition> packagePositions)
        {
            var isPackage = orderPosition.PricePosition.PricePositionEx.ShowPackagePositionsInAccount && packagePositions.Any();

            var pricePositionName = GetPricePositionName(orderPosition);

            var formulationBuilder = new StringBuilder();

            formulationBuilder.Append(
                string.Format("Размещение объявлений о наборе персонала в издании \"{0}\" г.{1} формат - {2} {3}",
                    orderPosition.Supplier.FullName,
                    orderPosition.Supplier.City.Name,
                    orderPosition.PricePosition.PricePositionType.Name,
                    pricePositionName));

            if (isPackage)
            {
                formulationBuilder.Append(" (");

                var packagePotionNames = new List<string>();
                foreach (var packagePosition in packagePositions)
                {
                    var packagePricePositionName = GetPricePositionName(packagePosition);
                    packagePotionNames.Add(
                        string.Format("{0} - {1} {2}",
                            packagePosition.Supplier.Company.Name,
                            packagePosition.Supplier.City.Name,
                            packagePricePositionName));
                }

                formulationBuilder.Append(string.Join(", ", packagePotionNames));
                formulationBuilder.Append(")");
            }

            formulationBuilder.Append(" ");

            var outs = string.Join(", ", orderPosition.GraphicPositions
                .Select(
                    gp =>
                        string.Format(
                            isPackage ? "({1})" : "№ {0}({1})",
                            gp.Graphic.Number, gp.Graphic.OutDate.ToString("dd.MM.yyyy"))));

            formulationBuilder.Append(outs);

            return formulationBuilder.ToString();
        }

        private string GetPricePositionName(OrderPosition orderPosition)
        {
            var builder = new StringBuilder();
            
            builder.Append(orderPosition.PricePosition.Name);

            if (orderPosition.PricePosition.PricePositionTypeId == 26)
                return builder.ToString();

            if (orderPosition.PricePosition.PricePositionType.EnableSecondSize)
            {
                builder.Append(
                    string.Format(" ({0}x{1} {2})",
                        orderPosition.PricePosition.FirstSize,
                        orderPosition.PricePosition.SecondSize,
                        orderPosition.PricePosition.Unit.Name)
                    );
            }
            else
            {
                builder.Append(
                    string.Format(" ({0} {1})",
                        orderPosition.PricePosition.FirstSize,
                        orderPosition.PricePosition.Unit.Name)
                    );
            }

            builder.Append(string.Format(" {0}", orderPosition.PricePosition.Description));

            return builder.ToString().Trim();
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
