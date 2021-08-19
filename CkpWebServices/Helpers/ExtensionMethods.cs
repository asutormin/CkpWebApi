using CkpDAL.Entities;
using CkpDAL.Entities.String;
using CkpModel.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices.Helpers
{
    public static class ExtensionMethods
    {
        public static List<SupplierInfoLight> ToLight(this List<SupplierInfo> suppliers)
        {
            var suppliersLight = new List<SupplierInfoLight>();

            foreach (var supplier in suppliers)
                suppliersLight.Add(supplier);

            return suppliersLight;
        }

        public static SupplierInfoLight ToLight(this SupplierInfo supplier)
        {
            var supplierLight = supplier as SupplierInfoLight;

            return supplierLight;
        }

        public static List<FormatTypeInfoLight> ToLight(this List<FormatTypeInfo> formatTypes)
        {
            var formatTypesLight = new List<FormatTypeInfoLight>();

            foreach (var formatType in formatTypes)
                formatTypesLight.Add(formatType);

            return formatTypesLight;
        }

        public static float GetTarifCost(this Price price)
        {
            var cost = price.Value * (1 + price.Markup / 100);

            return cost;
        }

        public static float GetClientPrice(this OrderPosition orderPosition)
        {
            var price = orderPosition.Price.Value *
                    (1 + orderPosition.Markup / 100) *
                    (1 + orderPosition.Nds / 100) *
                    (1 - orderPosition.Discount / 100);

            return price;
        }

        public static int GetQuantity(this OrderPosition orderPosition)
        {
            var quantity = orderPosition.GraphicPositions
                .Where(gp => gp.Id == gp.ParenGraphicPositiontId)
                .Sum(gp => gp.Count);

            return quantity;
        }

        public static float GetClientSum(this OrderPosition orderPosition)
        {
            var sum = orderPosition.GraphicPositions
                .Where(gp => gp.Id == gp.ParenGraphicPositiontId)
                .Sum(gp => gp.Count) *
                    orderPosition.Price.Value *
                    (1 + orderPosition.Markup / 100) *
                    (1 + orderPosition.Nds / 100) *
                    (1 - orderPosition.Discount / 100);

            return sum;
        }

        public static float GetClientNds(this OrderPosition orderPosition)
        {
            var nds = orderPosition.GraphicPositions.Sum(gp => gp.Count) *
                orderPosition.Price.Value *
                (1 + orderPosition.Markup / 100) *
                (orderPosition.Nds / 100) *
                (1 - orderPosition.Discount / 100);

            return nds;
        }

        public static IQueryable<OrderPositionInfo> SelectPositions(this IQueryable<OrderPosition> inputQuery)
        {
            var outputQuery = inputQuery
                .Select(
                    op =>
                        new OrderPositionInfo
                        {
                            Id = op.Id,
                            ParentId = op.ParentOrderPositionId,
                            ClientPrice = op.GetClientPrice(),
                            Quantity = op.GetQuantity(),
                            ClientSum = op.GetClientSum(),
                            Nds = op.GetClientNds(),
                            Supplier =
                                new SupplierInfoLight
                                {
                                    Id = op.Supplier.Id,
                                    Name = op.Supplier.Company.Name + " - " + op.Supplier.City.Name
                                },
                            Format =
                                new FormatInfo
                                {
                                    Id = op.PricePositionId,
                                    Name = op.PricePosition.Name,
                                    PackageLength = op.PricePosition.PackageLength,
                                    FirstSize = op.PricePosition.FirstSize,
                                    SecondSize = op.PricePosition.SecondSize,
                                    Version = op.PricePosition.BeginDate,
                                    Type =
                                        new FormatTypeInfoLight
                                        {
                                            Id = op.PricePosition.PricePositionTypeId,
                                            Name = op.PricePosition.PricePositionType.Name
                                        }
                                },
                            Graphics = op.GraphicPositions
                                .Select(
                                    gp =>
                                        new GraphicInfoLight
                                        {
                                            Id = gp.GraphicId,
                                            Number = gp.Graphic.Number,
                                            OutDate = gp.Graphic.OutDate
                                        })
                                .ToList(),
                            Rubrics = op.RubricPositions
                                .Select(
                                    rp =>
                                        new RubricInfoLight
                                        {
                                            Id = rp.RubricId,
                                            Number = rp.Rubric.Number,
                                            Name = rp.Rubric.Name
                                        })
                        });

            return outputQuery;
        }

        public static IEnumerable<GraphicPosition> GetChilds(this GraphicPosition graphicPosition)
        {
            var childs = graphicPosition.ChildGraphicPositions
                .Where(cgp => cgp.Id != cgp.ParenGraphicPositiontId)
                .AsEnumerable();

            return childs;
        }

        public static IEnumerable<StringPhone> GetActualItems(this IEnumerable<StringPhone> items)
        {
            var actualItems = items
                .Where(i => i.IsActual)
                .AsEnumerable();

            return actualItems;
        }

        public static IEnumerable<StringWeb> GetActualItems(this IEnumerable<StringWeb> items)
        {
            var actualItems = items
                .Where(i => i.IsActual)
                .AsEnumerable();

            return actualItems;
        }

        public static IEnumerable<StringAddress> GetActualItems(this IEnumerable<StringAddress> items)
        {
            var actualItems = items
                .Where(i => i.IsActual)
                .AsEnumerable();

            return actualItems;
        }

        public static IEnumerable<StringOccurrence> GetActualItems(this IEnumerable<StringOccurrence> items)
        {
            var now = DateTime.Now;

            var actualItems = items
                .Where(
                    i =>
                        i.BeginDate <= now &&
                        i.EndDate >= now)
                .AsEnumerable();

            return actualItems;
        }

        public static DateTime? GetMaxExitDate(this IEnumerable<OrderPosition> orderPositions)
        {
            var maxExitDate = orderPositions.Count() == 0
                ? (DateTime?)null
                : orderPositions
                    .SelectMany(op => op.GraphicPositions)
                    .Select(gp => gp.Graphic)
                    .Max(g => g.OutDate);

            return maxExitDate;
        }

        public static float GetClientSum(this IEnumerable<OrderPosition> orderPositions)
        {
            var clientSum = orderPositions
                .Where(op => op.ParentOrderPositionId == null)
                .Sum(op => op.GetClientSum());

            return clientSum;
        }
    }
}
