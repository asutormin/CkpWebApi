using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using CkpServices.Interfaces;
using CkpDAL;
using CkpEntities.Output;
using CkpEntities.Configuration;
using CkpDAL.Model;
using CkpEntities.Output.String;
using CkpServices.Helpers;

namespace CkpWebApi.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly BPFinanceContext _context;

        private readonly int _priceBusinessUnitId;
        private readonly int _pricePermissionFlag;
        private readonly List<int> _supplierIds;

        private static List<SupplierInfo> _suppliers;

        public SupplierService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;

            _priceBusinessUnitId = appParamsAccessor.Value.PriceBusinessUnitId;
            _pricePermissionFlag = appParamsAccessor.Value.PricePermissionFlag;
            _supplierIds = appParamsAccessor.Value.Suppliers;

            //if (_suppliers == null)
            //    _suppliers = GetSuppliers();
        }

        List<SupplierLight> ISupplierService.GetSuppliers()
        {
            var suppliers = _context.Suppliers
                .Include(su => su.Company)
                .Include(su => su.City)
                .Where(su => _supplierIds.Contains(su.Id) && su.Id != 1761)
                .Select(
                    su =>
                        new SupplierLight
                        {
                            Id = su.Id,
                            Name = GetSupplierNameWithCity(su)
                        })
                .ToList();

            return suppliers;

            // Временно, пока не будет готов сервис
            //return _suppliers.Where(su => su.Id != 1761).ToList().ToLight();
        }

        SupplierLight ISupplierService.GetSupplier(int supplierId)
        {
            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId).ToLight();

            var supplier = _context.Suppliers
                .Include(su => su.Company)
                .Include(su => su.City)
                .Where(su => su.Id == supplierId)
                .Select(
                    su =>
                        new SupplierLight
                        {
                            Id = su.Id,
                            Name = GetSupplierNameWithCity(su)
                        })
                .Single();

            return supplier;
        }

        //List<RubricInfo> ISupplierService.GetRubrics(int supplierId)
        //{
        //    // Временно, пока не будет готов сервис
        //    //return _suppliers.Single(su => su.Id == supplierId)
        //    //    .Rubrics.ToList();
        //    return GetRubrics(supplierId);
        //}

        List<RubricInfo> ISupplierService.GetRubrics(int priceId)
        {
            var rubrics = new List<RubricInfo>();

            var price = _context.Prices
                .Include(p => p.PricePosition)
                .Include(p => p.SupplierProjectRelation).ThenInclude(spr => spr.SupplierProject)
                .Single(p => p.Id == priceId);

            var projectRelation = price.SupplierProjectRelation;
            var project = projectRelation == null ? null : projectRelation.SupplierProject;
            var projectId = project == null ? 0 : project.Id;

            // Находим все рубрики поставщика
            rubrics = _context.RubricsActual
                .Where(r => r.SupplierId == price.PricePosition.SupplierId)
                .Select(
                    r =>
                        new RubricInfo
                        {
                            Id = r.Id,
                            Number = r.Number,
                            Name = r.Name,
                            ParentId = r.ParentRubricId,
                            SupplierId = r.SupplierId,
                            OrderBy = r.OrderBy,
                            CanUse = true,
                            Version = r.BeginDate
                        })
                .ToList();

            // Если у цены есть проект с монопольно используемыми рубриками
            if (project != null && project.ExclusiveRubrics)
            {
                // Находим монопольно используемые рубрики проекта
                var projectRubricIds = _context.SupplierProjectRubrics
                    .Where(spr => spr.SupplierProjectId == projectId)
                    .Select(spr => spr.RubricId);

                // Отфильтровываем из общего списка рубрик
                rubrics = rubrics
                    .Where(r => projectRubricIds.Contains(r.Id))
                    .ToList();
            }

            // Отбираем рубрики, монопольно задействованные в проектах поставщика,
            // за исключением рубрик, входящих в проект цены.
            var disabledRubricIds = _context.SupplierProjectRubrics
                .Include(spr => spr.SupplierProject)
                .Where(
                    spr =>
                        spr.SupplierProject.SupplierId == price.PricePosition.SupplierId &&
                        spr.SupplierProjectId != projectId &&
                        spr.SupplierProject.ExclusiveRubrics)
                .Select(spr => spr.RubricId);

            foreach (var rubric in rubrics)
            {
                // Помечаем монопольные рубрики проектов как не активные
                if (disabledRubricIds.Contains(rubric.Id))
                    rubric.CanUse = false;

                // Помечаем родительские рубрики как не активные
                if (rubrics.Any(r => r.ParentId == rubric.Id))
                    rubric.CanUse = false;
            }

            return rubrics;
        }

        RubricInfo ISupplierService.GetRubricVersion(int rubricId, DateTime rubricVersion)
        {
            return _context.RubricsVersionable
                .Where(
                    r =>
                        r.Id == rubricId &&
                        r.BeginDate == rubricVersion)
                .Select(
                    r =>
                        new RubricInfo
                        {
                            Id = r.Id,
                            Number = r.Number,
                            Name = r.Name,
                            SupplierId = r.SupplierId,
                            Version = r.BeginDate
                        })
                .Single();
        }

        List<FormatTypeLight> ISupplierService.GetFormatTypes(int supplierId)
        {
            var now = DateTime.Now;

            var formatTypes = _context
                .PricePositionTypes
                .Include(ppt => ppt.PricePositions).ThenInclude(pp => pp.Prices)
                .Where(
                    ppt => ppt.PricePositions
                    .Where(
                        pp =>
                            pp.SupplierId == supplierId &&
                            pp.Prices
                                .Where(
                                    p =>
                                        p.BusinessUnitId == _priceBusinessUnitId &&
                                        p.ActualBegin <= now &&
                                        p.ActualEnd >= now &&
                                        (p.PermissionFlag & _pricePermissionFlag) > 0)
                                .Count() > 0)
                    .Count() > 0)
                .Select(
                    ppt => new FormatTypeLight
                    {
                        Id = ppt.Id,
                        Name = ppt.Name
                    })
                .ToList();

            return formatTypes;

            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId)
            //    .FormatTypes.ToList().ToLight();
        }

        FormatTypeLight ISupplierService.GetFormatType(int formatTypeId)
        {
            return _context.PricePositionTypes
                .Where(
                    ppt => ppt.Id == formatTypeId)
                .Select(
                    ppt =>
                        new FormatTypeLight
                        {
                            Id = ppt.Id,
                            Name = ppt.Name
                        })
                .Single();
        }

        IEnumerable<TariffInfo> ISupplierService.GetTariffs(int supplierId, int formatTypeId)
        {
            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId)
            //    .FormatTypes.Single(ft => ft.Id == formatTypeId)
            //    .Tariffs
            //    .Select(
            //        t =>
            //            new TariffInfo
            //            {
            //                Supplier = t.Supplier,
            //                Format = t.Format,
            //                Price = t.Price,
            //                PackageTariffs = t.PackageTariffs
            //            })
            //    .ToList();

            return GetTarifs(supplierId, formatTypeId);
        }

        TariffInfo ISupplierService.GetTariffVersion(int formatId, DateTime formatVersion, int priceId)
        {
            var pricePosition = _context.PricePositionsVersionable
                .Include(pp => pp.Supplier.Company)
                .Include(pp => pp.Supplier.City)
                .Include(pp => pp.PricePositionType)
                .Single(
                    pp =>
                        pp.Id == formatId &&
                        pp.BeginDate == formatVersion);

            return _context.Prices
                .Where(
                    p => p.Id == priceId && p.PricePositionId == formatId)
                .Select(
                    p =>
                        new TariffInfo
                        {
                            Supplier =
                                new SupplierLight
                                {
                                    Id = pricePosition.SupplierId,
                                    Name = GetSupplierNameWithCity(pricePosition.Supplier)
                                },
                            Format =
                                new FormatInfo
                                {
                                    Id = pricePosition.Id,
                                    Name = pricePosition.Name,
                                    PackageLength = p.PricePosition.PackageLength,
                                    FirstSize = pricePosition.FirstSize,
                                    SecondSize = pricePosition.SecondSize,
                                    Type =
                                        new FormatTypeLight
                                        {
                                            Id = pricePosition.PricePositionTypeId,
                                            Name = pricePosition.PricePositionType.Name
                                        },
                                    Version = pricePosition.BeginDate
                                },
                            Price =
                                new PriceInfo
                                {
                                    Id = p.Id,
                                    Value = p.GetTarifCost()
                                }
                        })
                .Single();
        }

        IEnumerable<TariffInfo> ISupplierService.GetPackageTariffs(int supplierId, int formatTypeId, int formatId)
        {
            var emptyPackageTariffs = new List<TariffInfo>();

            var supplier = _suppliers.SingleOrDefault(su => su.Id == supplierId);
            if (supplier == null) return emptyPackageTariffs;

            var formatType = supplier.FormatTypes.SingleOrDefault(ft => ft.Id == formatTypeId);
            if (formatType == null) return emptyPackageTariffs;

            var tariff = formatType.Tariffs.Single(t => t.Format.Id == formatId);
            if (tariff == null) return emptyPackageTariffs;

            var packageTariffs = tariff.PackageTariffs;
            if (packageTariffs == null) return emptyPackageTariffs;

            return packageTariffs
                .Select(
                    t =>
                        new TariffInfo
                        {
                            Supplier = t.Supplier,
                            Format = t.Format,
                            Price = t.Price
                        })
                .ToList();
            /*
            return _suppliers.Single(su => su.Id == supplierId)
                .FormatTypes.Single(ft => ft.Id == formatTypeId)
                .Tariffs
                .Single(t => t.Format.Id == formatId)
                .PackageTariffs
                .Select(
                    t =>
                        new TariffInfo
                        {
                            Supplier = t.Supplier,
                            Format = t.Format,
                            Price = t.Price
                        })
                .ToList();
            */
        }

        List<GraphicInfo> ISupplierService.GetGraphics(int supplierId, int formatTypeId)
        {
            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId)
            //    .FormatTypes.Single(ft => ft.Id == formatTypeId)
            //    .Graphics.ToList();

            return GetGraphics(supplierId, formatTypeId);
        }

        GraphicInfo ISupplierService.GetGraphic(int graphicId)
        {
            return _context.Graphics
                .Where(gr => gr.Id == graphicId)
                .Select(
                    gr =>
                        new GraphicInfo
                        {
                            Id = gr.Id,
                            Number = gr.Number,
                            ClosingDate = gr.ClosingDate,
                            DeliverDate = gr.DeliverDate,
                            OutDate = gr.OutDate
                        })
                .Single();
        }

        HandbookStorage ISupplierService.GetHandbooks(int supplierId, int formatTypeId)
        {
            return _suppliers.Single(su => su.Id == supplierId)
                .FormatTypes.Single(ft => ft.Id == formatTypeId)
                .Handbooks;
        }

        List<Education> ISupplierService.GetEducationsHandbook(int supplierId, int formatTypeId)
        {
            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId)
            //    .FormatTypes.Single(ft => ft.Id == formatTypeId)
            //    .Handbooks.Educations;

            return _context.Handbooks
                .Where(h => h.HandbookTypeId == 1)
                .Select(h => new Education { Id = h.Id, Name = h.HandbookValue })
                .ToList();
        }

        List<Experience> ISupplierService.GetExperiencesHandbook(int supplierId, int formatTypeId)
        {
            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId)
            //    .FormatTypes.Single(ft => ft.Id == formatTypeId)
            //    .Handbooks.Experiences;

            return _context.Handbooks
             .Where(h => h.HandbookTypeId == 2)
             .Select(h => new Experience { Id = h.Id, Name = h.HandbookValue })
             .ToList();
        }

        List<Currency> ISupplierService.GetCurrenciesHandbook(int supplierId, int formatTypeId)
        {
            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId)
            //    .FormatTypes.Single(ft => ft.Id == formatTypeId)
            //    .Handbooks.Currencies;

            return _context.Handbooks
                 .Where(h => h.HandbookTypeId == 6)
                 .Select(h => new Currency { Id = h.Id, Name = h.HandbookValue })
                 .ToList();
        }

        List<WorkGraphic> ISupplierService.GetWorkGraphicsHandbook(int supplierId, int formatTypeId)
        {
            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId)
            //    .FormatTypes.Single(ft => ft.Id == formatTypeId)
            //    .Handbooks.WorkGraphics;

            return _context.Handbooks
                 .Where(h => h.HandbookTypeId == 4)
                 .Select(h => new WorkGraphic { Id = h.Id, Name = h.HandbookValue })
                 .ToList();
        }

        List<Occurrence> ISupplierService.GetOccurrenciesHandbook(int supplierId, int formatTypeId)
        {
            // Временно, пока не будет готов сервис
            //return _suppliers.Single(su => su.Id == supplierId)
            //    .FormatTypes.Single(ft => ft.Id == formatTypeId)
            //    .Handbooks.Occurrences;

            if (supplierId == 1678)
            {
                return _context.HandbookRelations
                    .Join(_context.Metros,
                            h => new { MetroId = h.HandbookId, h.HandbookTypeId },
                            m => new { MetroId = m.Id, HandbookTypeId = m.TypeId },
                            (h, m) => new { Handbook = h, Metro = m })
                        .Where(hm => hm.Handbook.CompanyId == supplierId)
                        .Select(hm => new Occurrence { Id = hm.Metro.Id, Name = hm.Metro.Name, TypeId = hm.Metro.TypeId })
                    .Union(
                    _context.HandbookRelations
                        .Join(_context.Cities,
                            h => new { CityId = h.HandbookId, h.HandbookTypeId },
                            c => new { CityId = c.Id, HandbookTypeId = c.TypeId },
                            (h, c) => new { Handbook = h, City = c })
                        .Where(
                                hc =>
                                    hc.Handbook.CompanyId == supplierId &&
                                    hc.City.IsShowForDistribution &&
                                    (hc.City.Id < 1000001 || hc.City.Id > 1000004))
                        .Select(hc => new Occurrence { Id = hc.City.Id, Name = hc.City.Name, TypeId = hc.City.TypeId }))
                    .Union(
                        _context.Cities
                        .Where(
                            c =>
                                c.IsShowForDistribution &&
                                (c.Id == 120001 || // Российская Федерация
                                c.Id == 110005 || // Центральный Федеральный округ
                                c.Id == 1 || // Москва
                                c.Id == 100050 || // Московская обл
                                c.Id == 1001194 || // Москва(только для РДВ)
                                c.Id == 1001195)) // Московская обл(только для РДВ))
                        .Select(c => new Occurrence { Id = c.Id, Name = c.Name, TypeId = c.TypeId }))
                    .ToList();
            }
            else
            {
                return _context.Metros
                    .Select(m => new Occurrence { Id = m.Id, Name = m.Name, TypeId = m.TypeId })
                    .Union(
                        _context.Cities
                        .Where(
                            ct => ct.Id < 1001000 && ct.IsShowForDistribution)
                        .Select(
                            ct => new Occurrence { Id = ct.Id, Name = ct.Name, TypeId = ct.TypeId }))
                    .ToList();
            }
        }

        private List<SupplierInfo> GetSuppliers()
        {
            var suppliers = _context.Suppliers
                .Include(su => su.Company)
                .Include(su => su.City)
                .Where(su => _supplierIds.Contains(su.Id))
                .Select(
                    su =>
                        new SupplierInfo
                        {
                            Id = su.Id,
                            Name = GetSupplierNameWithCity(su)
                        })
                .ToList();

            foreach (var supplier in suppliers)
            {
                supplier.FormatTypes = GetFormatTypes(supplier.Id);
                supplier.Rubrics = GetRubrics(supplier.Id);
            }

            return suppliers;
        }

        private List<FormatTypeInfo> GetFormatTypes(int supplierId)
        {
            var now = DateTime.Now;

            var formatTypes = _context.PricePositionTypes
                .Select(
                    ft =>
                        new FormatTypeInfo
                        {
                            Id = ft.Id,
                            Name = ft.Name
                        })
                .ToList();

            foreach (var formatType in formatTypes)
            {
                formatType.Tariffs = GetTarifs(supplierId, formatType.Id);
                formatType.Graphics = GetGraphics(supplierId, formatType.Id);

                if (formatType.Id == 1)
                    formatType.Handbooks = GetHandbooks(supplierId);
            }

            // Удаляем типы форматов, у которых нет форматов с актуальными ценами
            formatTypes.RemoveAll(ft => ft.Tariffs.Count() == 0);

            return formatTypes;
        }

        private List<TariffInfo> GetTarifs(int supplierId, int formatTypeId)
        {
            var now = DateTime.Now;

            var tarifs = _context.Prices
                .Include(p => p.PricePosition).ThenInclude(pp => pp.PricePositionType)
                .Include(p => p.PricePosition.Supplier.Company)
                .Include(p => p.PricePosition.Supplier.City)
                .Where(
                    p =>
                        p.PricePosition.SupplierId == supplierId &&
                        p.PricePosition.PricePositionTypeId == formatTypeId &&

                        p.BusinessUnitId == _priceBusinessUnitId &&
                        p.ActualBegin <= now &&
                        p.ActualEnd >= now &&
                        (p.PermissionFlag & _pricePermissionFlag) > 0)
                .Select(
                        p =>
                            new TariffInfo
                            {
                                Supplier = new SupplierLight
                                {
                                    Id = p.PricePosition.SupplierId,
                                    Name = GetSupplierNameWithCity(p.PricePosition.Supplier)
                                },
                                Format = new FormatInfo
                                {
                                    Id = p.PricePositionId,
                                    Name = p.PricePosition.Name,
                                    PackageLength = p.PricePosition.PackageLength,
                                    FirstSize = p.PricePosition.FirstSize,
                                    SecondSize = p.PricePosition.SecondSize,
                                    Version = p.PricePosition.BeginDate,
                                    Type =
                                        new FormatTypeLight
                                        {
                                            Id = p.PricePosition.PricePositionTypeId,
                                            Name = p.PricePosition.PricePositionType.Name
                                        }
                                },
                                Price = new PriceInfo
                                {
                                    Id = p.Id,
                                    Value = p.GetTarifCost()
                                }
                            })
                .ToList();

            foreach (var tarif in tarifs)
            {
                var packageTarifs = GetPackageTarifs(tarif.Format.Id);
                tarif.PackageTariffs = packageTarifs;
            }

            return tarifs;
        }

        private List<TariffInfo> GetPackageTarifs(int formatId)
        {
            var now = DateTime.Now;

            var packageTarifs = _context.PackagePositions
                .Include(pkg => pkg.Price)
                .Include(pkg => pkg.PricePosition.PricePositionType)
                .Include(pkg => pkg.Price.PricePosition.Supplier.Company)
                .Include(pkg => pkg.Price.PricePosition.Supplier.City)
                .Where(
                    pkg =>
                        pkg.PricePositionId == formatId)
                .Select(
                    pkg =>
                        new TariffInfo
                        {
                            Supplier = new SupplierLight
                            {
                                Id = pkg.Price.PricePosition.Supplier.Id,
                                Name = GetSupplierNameWithCity(pkg.Price.PricePosition.Supplier)
                            },
                            Format = new FormatInfo
                            {
                                Id = pkg.Price.PricePositionId,
                                Name = pkg.Price.PricePosition.Name,
                                PackageLength = pkg.Price.PricePosition.PackageLength,
                                FirstSize = pkg.Price.PricePosition.FirstSize,
                                SecondSize = pkg.Price.PricePosition.SecondSize,
                                Version = pkg.Price.PricePosition.BeginDate,
                                Type =
                                    new FormatTypeLight
                                    {
                                        Id = pkg.Price.PricePosition.PricePositionTypeId,
                                        Name = pkg.Price.PricePosition.PricePositionType.Name
                                    }
                            },
                            Price = new PriceInfo
                            {
                                Id = pkg.Price.Id,
                                Value = pkg.Price.GetTarifCost()
                            }
                        })
                .ToList();

            return packageTarifs;
        }

        private List<GraphicInfo> GetGraphics(int supplierId, int pricePositionTypeId)
        {
            var now = DateTime.Now;
            var date999 = new DateTime(2099, 12, 31);

            var graphics = _context.Graphics
                .Where(
                    gr =>
                        gr.SupplierId == supplierId &&
                        gr.PricePositionTypeId == pricePositionTypeId &&
                        gr.DeliverDate.Date >= now.Date &&
                        gr.Number != "ПЗ" &&
                        gr.DeliverDate < date999 && gr.ClosingDate < date999 && gr.OutDate < date999 && gr.FinishDate < date999 &&
                        (supplierId == 1678 && pricePositionTypeId != 26 && gr.Description == "РДВ-медиа" || supplierId == 1678 && pricePositionTypeId == 26 || supplierId != 1678))
                .Select(
                    gr =>
                        new GraphicInfo
                        {
                            Id = gr.Id,
                            Number = gr.Number,
                            DeliverDate = gr.DeliverDate,
                            ClosingDate = gr.ClosingDate,
                            OutDate = gr.OutDate,
                            FinishDate = gr.FinishDate
                        })
                .ToList();

            return graphics;
        }

        private List<RubricInfo> GetRubrics(int supplierId)
        {
            var now = DateTime.Now;

            var rubrics = _context.RubricsActual
                .Where(
                    r =>
                        r.SupplierId == supplierId)
                .Select(
                    r => new RubricInfo
                    {
                        Id = r.Id,
                        ParentId = r.ParentRubricId,
                        SupplierId = r.SupplierId,
                        Number = r.Number,
                        Name = r.Name,
                        OrderBy = r.OrderBy,
                        CanUse = true,
                        Version = r.BeginDate
                    })
                .OrderBy(r => r.OrderBy)
                .ToList();

            foreach (var rubric in rubrics)
            {
                if (rubrics.Any(r => r.ParentId != null && r.ParentId == rubric.Id))
                    rubric.CanUse = false;
            }

            return rubrics;
        }

        private HandbookStorage GetHandbooks(int supplierId)
        {
            var handbooks = new HandbookStorage();

            handbooks.Educations = _context.Handbooks
                .Where(h => h.HandbookTypeId == 1)
                .Select(h => new Education { Id = h.Id, Name = h.HandbookValue })
                .ToList();

            handbooks.Experiences = _context.Handbooks
                 .Where(h => h.HandbookTypeId == 2)
                 .Select(h => new Experience { Id = h.Id, Name = h.HandbookValue })
                 .ToList();

            handbooks.WorkGraphics = _context.Handbooks
                 .Where(h => h.HandbookTypeId == 4)
                 .Select(h => new WorkGraphic { Id = h.Id, Name = h.HandbookValue })
                 .ToList();

            handbooks.Currencies = _context.Handbooks
                 .Where(h => h.HandbookTypeId == 6)
                 .Select(h => new Currency { Id = h.Id, Name = h.HandbookValue })
                 .ToList();

            if (supplierId == 1678)
            {
                handbooks.Occurrences =
                    _context.HandbookRelations
                    .Join(_context.Metros,
                            h => new { MetroId = h.HandbookId, h.HandbookTypeId },
                            m => new { MetroId = m.Id, HandbookTypeId = m.TypeId },
                            (h, m) => new { Handbook = h, Metro = m })
                        .Where(hm => hm.Handbook.CompanyId == supplierId)
                        .Select(hm => new Occurrence { Id = hm.Metro.Id, Name = hm.Metro.Name, TypeId = hm.Metro.TypeId })
                    .Union(
                    _context.HandbookRelations
                        .Join(_context.Cities,
                            h => new { CityId = h.HandbookId, h.HandbookTypeId },
                            c => new { CityId = c.Id, HandbookTypeId = c.TypeId },
                            (h, c) => new { Handbook = h, City = c })
                        .Where(
                                hc =>
                                    hc.Handbook.CompanyId == supplierId &&
                                    hc.City.IsShowForDistribution &&
                                    (hc.City.Id < 1000001 || hc.City.Id > 1000004))
                        .Select(hc => new Occurrence { Id = hc.City.Id, Name = hc.City.Name, TypeId = hc.City.TypeId }))
                    .Union(
                        _context.Cities
                        .Where(
                            c =>
                                c.IsShowForDistribution &&
                                (c.Id == 120001 || // Российская Федерация
                                c.Id == 110005 || // Центральный Федеральный округ
                                c.Id == 1 || // Москва
                                c.Id == 100050 || // Московская обл
                                c.Id == 1001194 || // Москва(только для РДВ)
                                c.Id == 1001195)) // Московская обл(только для РДВ))
                        .Select(c => new Occurrence { Id = c.Id, Name = c.Name, TypeId = c.TypeId }))
                    .ToList();
            }
            else
            {
                handbooks.Occurrences = _context.Metros
                    .Select(m => new Occurrence { Id = m.Id, Name = m.Name, TypeId = m.TypeId })
                    .Union(
                        _context.Cities
                        .Where(
                            ct => ct.Id < 1001000 && ct.IsShowForDistribution)
                        .Select(
                            ct => new Occurrence { Id = ct.Id, Name = ct.Name, TypeId = ct.TypeId }))
                    .ToList();
            }

            return handbooks;
        }

        private static string GetSupplierNameWithCity(Supplier supplier)
        {
            var name = supplier.Company.Name + " - " + supplier.City.Name;

            return name;
        }
    }
}
