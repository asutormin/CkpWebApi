using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using CkpServices.Interfaces;
using CkpDAL;
using CkpModel.Output;
using CkpDAL.Entities;
using CkpModel.Output.String;
using CkpServices.Helpers;
using CkpInfrastructure.Configuration;
using CkpServices.Processors.Interfaces;
using CkpDAL.Repository;
using CkpServices.Processors;

namespace CkpWebApi.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly BPFinanceContext _context;

        private readonly IHandbookProcessor _handbookProcessor;

        private readonly int[] _supplierIds;
        
        private readonly int _pricePermissionFlag;

        public SupplierService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            _handbookProcessor = new HandbookProcessor(
                _context,
                repository);

            _supplierIds = appParamsAccessor.Value.SupplierIds;
            
            _pricePermissionFlag = appParamsAccessor.Value.PricePermissionFlag;
        }

        #region Suppliers

        public List<SupplierInfoLight> GetSuppliers()
        {
            var suppliers = _context.Suppliers
                .Include(su => su.Company)
                .Include(su => su.City)
                .Where(su => _supplierIds.Contains(su.Id) && su.Id != 1761)
                .Select(
                    su =>
                        new SupplierInfoLight
                        {
                            Id = su.Id,
                            Name = GetSupplierNameWithCity(su)
                        })
                .ToList();

            return suppliers;
        }

        public SupplierInfoLight GetSupplier(int supplierId)
        {
            var supplier = _context.Suppliers
                .Include(su => su.Company)
                .Include(su => su.City)
                .Where(su => su.Id == supplierId)
                .Select(
                    su =>
                        new SupplierInfoLight
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

                return rubrics;
            }

            // Отбираем рубрики, монопольно задействованные в проектах поставщика,
            // за исключением рубрик, входящих в проект цены.
            var disabledRubricIds = _context.SupplierProjectRubrics
                .Include(spr => spr.SupplierProject)
                .Where(
                    spr =>
                        spr.SupplierProject.SupplierId == price.PricePosition.SupplierId &&
                        spr.SupplierProject.Id != projectId &&
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

        public List<FormatTypeInfoLight> GetFormatTypes(int supplierId)
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
                                        p.ActualBegin <= now &&
                                        p.ActualEnd >= now &&
                                        (p.PermissionFlag & _pricePermissionFlag) > 0)
                                .Count() > 0)
                    .Count() > 0)
                .Select(
                    ppt => new FormatTypeInfoLight
                    {
                        Id = ppt.Id,
                        Name = ppt.Name
                    })
                .ToList();

            return formatTypes;
        }

        public FormatTypeInfoLight GetFormatType(int formatTypeId)
        {
            return _context.PricePositionTypes
                .Where(
                    ppt => ppt.Id == formatTypeId)
                .Select(
                    ppt =>
                        new FormatTypeInfoLight
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
                .Include(p => p.PricePosition).ThenInclude(pp => pp.Supplier).ThenInclude(su => su.Company)
                .Include(p => p.PricePosition).ThenInclude(pp => pp.Supplier).ThenInclude(su => su.City)
                .Include(p => p.SupplierProjectRelation).ThenInclude(spr => spr.SupplierProject)
                .Where(
                    p =>
                        p.PricePosition.SupplierId == supplierId &&
                        p.PricePosition.PricePositionTypeId == formatTypeId &&
                        p.ActualBegin <= now &&
                        p.ActualEnd >= now &&
                        (p.PermissionFlag & _pricePermissionFlag) > 0)
                .OrderBy(p => p.PricePosition.PricePositionEx.OrderBy)
                .Select(
                        p =>
                            new TariffInfo
                            {
                                Supplier = new SupplierInfoLight
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
                                    Type = new FormatTypeInfoLight
                                    {
                                        Id = p.PricePosition.PricePositionTypeId,
                                        Name = p.PricePosition.PricePositionType.Name
                                    }
                                },
                                Price = new PriceInfo
                                {
                                    Id = p.Id,
                                    BusinessUnitId = p.BusinessUnitId,
                                    Value = p.GetTarifCost()
                                },
                                Project = p.SupplierProjectRelation == null
                                    ? null
                                    : p.SupplierProjectRelation.SupplierProject == null
                                        ? null
                                        : new ProjectInfo
                                        {
                                            Id = p.SupplierProjectRelation.SupplierProject.Id,
                                            Name = p.SupplierProjectRelation.SupplierProject.Name
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
                                new SupplierInfoLight
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
                                        new FormatTypeInfoLight
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
                                    BusinessUnitId = p.BusinessUnitId,
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
                            Supplier = new SupplierInfoLight
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
                                    new FormatTypeInfoLight
                                    {
                                        Id = pkg.Price.PricePosition.PricePositionTypeId,
                                        Name = pkg.Price.PricePosition.PricePositionType.Name
                                    }
                            },
                            Price = new PriceInfo
                            {
                                Id = pkg.Price.Id,
                                BusinessUnitId = pkg.Price.BusinessUnitId,
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

        public List<EducationInfo> GetEducations()
        {
            return _handbookProcessor.GetEducations()
                .Select(h => new EducationInfo { Id = h.Id, Name = h.HandbookValue })
                .ToList();
        }

        public List<ExperienceInfo> GetExperiences()
        {
            return _handbookProcessor.GetExperiences()
             .Select(h => new ExperienceInfo { Id = h.Id, Name = h.HandbookValue })
             .ToList();
        }

        public List<WorkGraphicInfo> GetWorkGraphics()
        {
            return _handbookProcessor.GetWorkGraphics()
                 .Select(h => new WorkGraphicInfo { Id = h.Id, Name = h.HandbookValue })
                 .ToList();
        }

        public List<CurrencyInfo> GetCurrencies()
        {
            return _handbookProcessor.GetCurrencies()
                 .Where(h => h.Id == 23 || h.Id == 29) // RUR и т.р.
                 .Select(h => new CurrencyInfo { Id = h.Id, Name = h.HandbookValue })
                 .ToList();
        }

        public List<OccurrenceInfo> GetOccurrencies(int supplierId)
        {
            if (supplierId == 1678)
            {
                return _context.HandbookRelations
                    .Join(_context.Metros,
                            h => new { MetroId = h.HandbookId, h.HandbookTypeId },
                            m => new { MetroId = m.Id, HandbookTypeId = m.TypeId },
                            (h, m) => new { Handbook = h, Metro = m })
                        .Where(hm => hm.Handbook.CompanyId == supplierId)
                        .Select(hm => new OccurrenceInfo { Id = hm.Metro.Id, Name = hm.Metro.Name, TypeId = hm.Metro.TypeId })
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
                        .Select(hc => new OccurrenceInfo { Id = hc.City.Id, Name = hc.City.Name, TypeId = hc.City.TypeId }))
                    .OrderBy(o => o.Name)
                    .ToList();
            }
            else
            {
                return _context.Metros
                    .Select(m => new OccurrenceInfo { Id = m.Id, Name = m.Name, TypeId = m.TypeId })
                    .Union(
                        _context.Cities
                        .Where(
                            ct => ct.Id < 1001000 && ct.IsShowForDistribution)
                        .Select(
                            ct => new OccurrenceInfo { Id = ct.Id, Name = ct.Name, TypeId = ct.TypeId }))
                    .ToList();
            }
        }

        #endregion

        private static string GetSupplierNameWithCity(Supplier supplier)
        {
            var name = supplier.Company.Name.Replace(" регионы", "") + " - " + supplier.City.Name;

            return name;
        }
    }
}
