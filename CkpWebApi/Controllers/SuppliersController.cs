using System.Collections.Generic;
using CkpWebApi.OutputEntities;
using Microsoft.AspNetCore.Mvc;
using CkpWebApi.Services.Interfaces;
using CkpWebApi.OutputEntities.String;
using System;

namespace CkpWebApi.Controllers
{
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
        public IEnumerable<SupplierLight> GetSuppliers()
        {
            var suppliers = _supplierService.GetSuppliers();

            return suppliers;
        }

        [HttpGet("{supplierId}")]
        public SupplierLight GetSupplier(int supplierId)
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
        public IEnumerable<FormatTypeLight> GetFormatTypes(int supplierId)
        {
            var formatTypes = _supplierService.GetFormatTypes(supplierId);

            return formatTypes;
        }

        [HttpGet("formatTypes/{formatTypeId}")]
        public FormatTypeLight GetFormatType(int formatTypeId)
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

        [HttpGet("{supplierId}/packagetariffs/{formatTypeId}/{formatId}")]
        public IEnumerable<TariffInfo> GetPackageTariffs(int supplierId, int formatTypeId, int formatId)
        {
            var packageTariffs = _supplierService.GetPackageTariffs(supplierId, formatTypeId, formatId);

            return packageTariffs;
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

        [HttpGet("{supplierId}/handbooks/{formatTypeId}")]
        public HandbookStorage GetHandbooks(int supplierId, int formatTypeId)
        {
            var handbooks = _supplierService.GetHandbooks(supplierId, formatTypeId);

            return handbooks;
        }

        [HttpGet("{supplierId}/handbooks/{formatTypeId}/educations")]
        public IEnumerable<Education> GetEducationsHandbook(int supplierId, int formatTypeId)
        {
            var educations = _supplierService.GetEducationsHandbook(supplierId, formatTypeId);

            return educations;
        }

        [HttpGet("{supplierId}/handbooks/{formatTypeId}/experiences")]
        public IEnumerable<Experience> GetExperiencesHandbook(int supplierId, int formatTypeId)
        {
            var experiences = _supplierService.GetExperiencesHandbook(supplierId, formatTypeId);

            return experiences;
        }

        [HttpGet("{supplierId}/handbooks/{formatTypeId}/currencies")]
        public IEnumerable<Currency> GetCurrenciesHandbook(int supplierId, int formatTypeId)
        {
            var currencies = _supplierService.GetCurrenciesHandbook(supplierId, formatTypeId);

            return currencies;
        }

        [HttpGet("{supplierId}/handbooks/{formatTypeId}/workgraphics")]
        public IEnumerable<WorkGraphic> GetWorkGraphicsHandbook(int supplierId, int formatTypeId)
        {
            var workGraphics = _supplierService.GetWorkGraphicsHandbook(supplierId, formatTypeId);

            return workGraphics;
        }

        [HttpGet("{supplierId}/handbooks/{formatTypeId}/occurrencies")]
        public IEnumerable<Occurrence> GetOccurrenciesHandbook(int supplierId, int formatTypeId)
        {
            var occurrencies = _supplierService.GetOccurrenciesHandbook(supplierId, formatTypeId);

            return occurrencies;
        }
    }
}
