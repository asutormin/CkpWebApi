using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using CkpServices.Interfaces;
using CkpModel.Output;
using CkpModel.Output.String;

namespace CkpWebApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public IEnumerable<SupplierInfoLight> GetSuppliers()
        {
            var suppliers = _supplierService.GetSuppliers();

            return suppliers;
        }

        [HttpGet("{supplierId}")]
        public SupplierInfoLight GetSupplier(int supplierId)
        {
            var supplier = _supplierService.GetSupplier(supplierId);

            return supplier;
        }

        [HttpGet("price/{priceId}/rubrics")]
        public IEnumerable<RubricInfo> GetRubrics(int priceId)
        {
            var rubrics = _supplierService.GetRubrics(priceId);

            return rubrics;
        }

        [HttpGet("rubrics/{rubricId}/{rubricVersion}")]
        public RubricInfo GetRubricVersion(int rubricId, DateTime rubricVersion)
        {
            var rubric = _supplierService.GetRubricVersion(rubricId, rubricVersion);

            return rubric;
        }

        [HttpGet("{supplierId}/formatTypes")]
        public IEnumerable<FormatTypeInfoLight> GetFormatTypes(int supplierId)
        {
            var formatTypes = _supplierService.GetFormatTypes(supplierId);

            return formatTypes;
        }

        [HttpGet("formatTypes/{formatTypeId}")]
        public FormatTypeInfoLight GetFormatType(int formatTypeId)
        {
            var formatType = _supplierService.GetFormatType(formatTypeId);

            return formatType;
        }

        [HttpGet("{supplierId}/tariffs/{formatTypeId}")]
        public IEnumerable<TariffInfo> GetTariffs(int supplierId, int formatTypeId)
        {
            var tariffs = _supplierService.GetTariffs(supplierId, formatTypeId);

            return tariffs;
        }

        [HttpGet("tariffs/{formatId}/{formatVersion}/{priceId}")]
        public TariffInfo GetTariffVersion(int formatId, DateTime formatVersion, int priceId)
        {
            var tariff = _supplierService.GetTariffVersion(formatId, formatVersion, priceId);

            return tariff;
        }

        [HttpGet("{supplierId}/graphics/{formatTypeId}")]
        public IEnumerable<GraphicInfo> GetGraphics(int supplierId, int formatTypeId)
        {
            var graphics = _supplierService.GetGraphics(supplierId, formatTypeId);

            return graphics;
        }

        [HttpGet("graphics/{graphicId}")]
        public GraphicInfo GetGraphic(int graphicId)
        {
            var graphic = _supplierService.GetGraphic(graphicId);

            return graphic;
        }

        [HttpGet("handbooks/educations")]
        public IEnumerable<EducationInfo> GetEducations()
        {
            var educations = _supplierService.GetEducations();

            return educations;
        }

        [HttpGet("handbooks/experiences")]
        public IEnumerable<ExperienceInfo> GetExperiences()
        {
            var experiences = _supplierService.GetExperiences();

            return experiences;
        }

        [HttpGet("handbooks/currencies")]
        public IEnumerable<CurrencyInfo> GetCurrenciesHandbook()
        {
            var currencies = _supplierService.GetCurrencies();

            return currencies;
        }

        [HttpGet("handbooks/workgraphics")]
        public IEnumerable<WorkGraphicInfo> GetWorkGraphicsHandbook()
        {
            var workGraphics = _supplierService.GetWorkGraphics();

            return workGraphics;
        }

        [HttpGet("handbooks/occurrencies/{supplierId}")]
        public IEnumerable<OccurrenceInfo> GetOccurrenciesHandbook(int supplierId)
        {
            var occurrencies = _supplierService.GetOccurrencies(supplierId);

            return occurrencies;
        }
    }
}
