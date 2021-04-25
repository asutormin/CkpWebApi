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

        public SupplierService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;

            _priceBusinessUnitId = appParamsAccessor.Value.PriceBusinessUnitId;
            _pricePermissionFlag = appParamsAccessor.Value.PricePermissionFlag;
            _supplierIds = appParamsAccessor.Value.Suppliers;
        }

        #region Suppliers

        public List<SupplierLight> GetSuppliers()
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
        }

        public SupplierLight GetSupplier(int supplierId)
        {
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

        #endregion

        #region Rubrics

        public List<RubricInfo> GetRubrics(int priceId)
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
                .OrderBy(r => r.OrderBy)
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
                    .OrderBy(r => r.OrderBy)
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

        public RubricInfo GetRubricVersion(int rubricId, DateTime rubricVersion)
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

        #endregion

        #region FormatTypes

        public List<FormatTypeLight> GetFormatTypes(int supplierId)
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
        }

        public FormatTypeLight GetFormatType(int formatTypeId)
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

        #endregion

        #region Tariffs

        public IEnumerable<TariffInfo> GetTariffs(int supplierId, int formatTypeId)
        {
            var now = DateTime.Now;

            var tarifs = _context.Prices
                .Include(p => p.PricePosition).ThenInclude(pp => pp.PricePositionEx)
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
                .OrderBy(p => p.PricePosition.PricePositionEx.OrderBy)
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
                                    EnableSecondSize = p.PricePosition.PricePositionType.EnableSecondSize,
                                    UnitName = p.PricePosition.Unit.Name,
                                    Description = p.PricePosition.Description,
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

        public TariffInfo GetTariffVersion(int formatId, DateTime formatVersion, int priceId)
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

        #endregion

        #region Graphics

        public List<GraphicInfo> GetGraphics(int supplierId, int formatTypeId)
        {
            var now = DateTime.Now;
            var date999 = new DateTime(2099, 12, 31);

            var graphics = _context.Graphics
                .Where(
                    gr =>
                        gr.SupplierId == supplierId &&
                        gr.PricePositionTypeId == formatTypeId &&
                        gr.DeliverDate.Date >= now.Date &&
                        gr.Number != "ПЗ" &&
                        gr.DeliverDate < date999 && gr.ClosingDate < date999 && gr.OutDate < date999 && gr.FinishDate < date999 &&
                        (supplierId == 1678 && formatTypeId != 26 && gr.Description == "РДВ-медиа" || supplierId == 1678 && formatTypeId == 26 || supplierId != 1678))
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

        public GraphicInfo GetGraphic(int graphicId)
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

        #endregion

        #region Handbooks

        public List<Education> GetEducationsHandbook(int supplierId, int formatTypeId)
        {
            return _context.Handbooks
                .Where(h => h.HandbookTypeId == 1)
                .Select(h => new Education { Id = h.Id, Name = h.HandbookValue })
                .ToList();
        }

        public List<Experience> GetExperiencesHandbook(int supplierId, int formatTypeId)
        {
            return _context.Handbooks
             .Where(h => h.HandbookTypeId == 2)
             .Select(h => new Experience { Id = h.Id, Name = h.HandbookValue })
             .ToList();
        }

        public List<Currency> GetCurrenciesHandbook(int supplierId, int formatTypeId)
        {
            return _context.Handbooks
                 .Where(h => h.HandbookTypeId == 6)
                 .Select(h => new Currency { Id = h.Id, Name = h.HandbookValue })
                 .ToList();
        }

        public List<WorkGraphic> GetWorkGraphicsHandbook(int supplierId, int formatTypeId)
        {
            return _context.Handbooks
                 .Where(h => h.HandbookTypeId == 4)
                 .Select(h => new WorkGraphic { Id = h.Id, Name = h.HandbookValue })
                 .ToList();
        }

        public List<Occurrence> GetOccurrenciesHandbook(int supplierId, int formatTypeId)
        {
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
                    .OrderBy(o => o.Name)
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

        #endregion

        private static string GetSupplierNameWithCity(Supplier supplier)
        {
            var name = supplier.Company.Name + " - " + supplier.City.Name;

            return name;
        }
    }
}
