using CkpDAL;
using CkpDAL.Repository;
using CkpInfrastructure.Configuration;
using CkpModel.Output;
using CkpServices.Helpers;
using CkpServices.Helpers.Providers;
using CkpServices.Interfaces;
using CkpServices.Processors;
using CkpServices.Processors.Interfaces;
using CkpServices.Processors.String;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices
{
    public class SearchService : ISearchService
    {
        private readonly BPFinanceContext _context;
        private readonly IOrderPositionProcessor _orderPositionProcessor;

        public SearchService(BPFinanceContext context, IOptions<AppSettings> appSettingsAccessor, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            var businessUnitIdByPriceIdProvider = new BusinessUnitIdByPriceIdProvider(_context);

            var rubricProcessor = new RubricProcessor(
                _context,
                repository);
            var graphicProcessor = new GraphicProcessor(
                _context,
                repository);
            var orderImProcessor = new OrderImProcessor(
                _context,
                repository);
            var stringProcessor = new StringProcessor(
                _context,
                repository);
            var moduleProcessor = new ModuleProcessor(
                appSettingsAccessor.Value.OrderImFolderTemplate,
                appSettingsAccessor.Value.DatabaseName);
            var positionImProcessor = new PositionImProcessor(
                _context,
                repository,
                orderImProcessor,
                stringProcessor,
                moduleProcessor);

            _orderPositionProcessor = new OrderPositionProcessor(
                _context,
                repository,
                rubricProcessor,
                graphicProcessor,
                positionImProcessor,
                appParamsAccessor.Value.BasketOrderDescription);
        }

        public List<OrderPositionInfo> Search(int clientLegalPersonId, string value, int skipCount)
        {
            value = value == null ? string.Empty : value.ToUpper();

            int pageSize = 18;

            var orderPositionIds = _context.OrderPositions
                .Include(op => op.Order)
                //.Include(op => op.Supplier).ThenInclude(su => su.Company)
                //.Include(op => op.Supplier).ThenInclude(su => su.City)
                .Include(op => op.PricePosition).ThenInclude(pp => pp.PricePositionType)
                .Include(op => op.RubricPositions).ThenInclude(rp => rp.Rubric)
                .Include(op => op.GraphicPositions).ThenInclude(gp => gp.Graphic)
                .Include(op => op.PositionIm).ThenInclude(pim => pim.StringPosition).ThenInclude(sp => sp.Occurrences)
                .Include(op => op.PositionIm).ThenInclude(pim => pim.StringPosition).ThenInclude(sp => sp.Addresses)
                .Include(op => op.PositionIm).ThenInclude(pim => pim.StringPosition).ThenInclude(sp => sp.Webs)
                .Where(
                    op =>
                        op.Order.OrderDate.Year > 2018 &&
                        op.Order.ActivityTypeId == 1 &&
                        op.Order.ClientLegalPersonId == clientLegalPersonId &&
                        (
                            // op.Supplier.Company.Name.ToUpper().Contains(value) ||
                            // op.Supplier.City.Name.ToUpper().Contains(value) ||
                            // op.PricePosition.Name.ToUpper().Contains(value) ||
                            op.PricePosition.PricePositionType.Name.ToUpper().Contains(value) ||

                            (
                                op.RubricPositions != null &&
                                (
                                    op.RubricPositions.Any(rp => rp.Rubric.Number.Contains(value)) ||
                                    op.RubricPositions.Any(rp => rp.Rubric.Name.ToUpper().Contains(value))
                                )
                            ) ||
                            /*
                            (
                                op.GraphicPositions != null && op.GraphicPositions
                                    .Any(gp => (gp.Graphic.OutDate.Day.ToString() + "." + gp.Graphic.OutDate.Month.ToString())
                                        .Contains(value))
                            ) ||
                            */
                            (
                                op.PositionIm != null && op.PositionIm.StringPosition != null &&
                                (
                                    op.PositionIm.StringPosition.AnonymousCompanyName.ToUpper().Contains(value) ||
                                    op.PositionIm.StringPosition.VacancyName.ToUpper().Contains(value) ||
                                    op.PositionIm.StringPosition.VacancyAdditional.ToUpper().Contains(value) ||
                                    op.PositionIm.StringPosition.Requirement.ToUpper().Contains(value) ||
                                    op.PositionIm.StringPosition.Responsibility.ToUpper().Contains(value) ||
                                    op.PositionIm.StringPosition.Condition.ToUpper().Contains(value) ||
                                    // op.PositionIm.StringPosition.Addresses
                                    //     .Any(addr => addr.Description.ToUpper().Contains(value)) ||
                                    op.PositionIm.StringPosition.Phones
                                        .Any(ph => (ph.Code + ph.Number).Contains(value)) ||
                                    op.PositionIm.StringPosition.Phones
                                        .Any(ph => ph.Description.ToUpper().Contains(value))
                                )
                            )
                        )
                    )
                .Select(op => op.ParentOrderPositionId == null ? op.Id : (int)op.ParentOrderPositionId)
                .Distinct()
                .OrderByDescending(id => id).AsEnumerable()
                .Select((id, index) => new { id, index })
                .Where(val => val.index >= skipCount)
                .Take(pageSize)
                .Select(val => val.id)
                .ToArray();

            var orderPositions = _orderPositionProcessor.GetOrderPositionsByIdsOuery(clientLegalPersonId, orderPositionIds)
                .SelectPositions()
                .OrderByDescending(op => op.Id)
                .ToList();

            return orderPositions;
        }
    }
}
