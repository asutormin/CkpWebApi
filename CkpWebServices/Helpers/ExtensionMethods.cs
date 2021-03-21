﻿using CkpDAL.Model;
using CkpDAL.Model.String;
using CkpEntities.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices.Helpers
{
    public static class ExtensionMethods
    {
        public static List<SupplierLight> ToLight(this List<SupplierInfo> suppliers)
        {
            var suppliersLight = new List<SupplierLight>();

            foreach (var supplier in suppliers)
                suppliersLight.Add(supplier);

            return suppliersLight;
        }

        public static SupplierLight ToLight(this SupplierInfo supplier)
        {
            var supplierLight = supplier as SupplierLight;

            return supplierLight;
        }

        public static List<FormatTypeLight> ToLight(this List<FormatTypeInfo> formatTypes)
        {
            var formatTypesLight = new List<FormatTypeLight>();

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

        public static IQueryable<PositionInfo> SelectPositions(this IQueryable<OrderPosition> inputQuery)
        {
            var outputQuery = inputQuery
                .Select(
                    op =>
                        new PositionInfo
                        {
                            Id = op.Id,
                            ParentId = op.ParentOrderPositionId,
                            ClientPrice = op.GetClientPrice(),
                            Quantity = op.GetQuantity(),
                            ClientSum = op.GetClientSum(),
                            Nds = op.GetClientNds(),
                            Supplier =
                                new SupplierLight
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
                                        new FormatTypeLight
                                        {
                                            Id = op.PricePosition.PricePositionTypeId,
                                            Name = op.PricePosition.PricePositionType.Name
                                        }
                                },
                            Graphics = op.GraphicPositions
                                .Select(
                                    gp =>
                                        new GraphicLight
                                        {
                                            Id = gp.GraphicId,
                                            Number = gp.Graphic.Number,
                                            OutDate = gp.Graphic.OutDate
                                        })
                                .ToList(),
                            Rubrics = op.RubricPositions
                                .Select(
                                    rp =>
                                        new RubricLight
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

        public static string GetAccountPositionName(this OrderPosition orderPosition)
        {
            var pricePositionName = orderPosition.PricePosition.PricePositionType.IsEnableSecondSize
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

        public static string GetAccountNumenclature(this OrderPosition orderPosition)
        {
            string nomenclature;

            if (orderPosition.PricePosition.IsShowSize)
            {
                nomenclature = orderPosition.PricePosition.PricePositionType.IsEnableSecondSize
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

        public static int GetPositionsCount(this OrderPosition orderPosition)
        {
            var count = orderPosition.GraphicPositions
                .Where(gp => gp.Id == gp.ParenGraphicPositiontId)
                .Sum(gp => gp.Count);

            return count;
        }

        public static DateTime GetFirstOutDate(this OrderPosition orderPosition)
        {
            var firstOutDate = orderPosition.GraphicPositions
                .Select(gp => gp.Graphic.OutDate)
                .Min();

            return firstOutDate;
        }
    }
}