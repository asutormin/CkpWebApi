using CkpModel.Output;
using CkpModel.Output.String;
using System;
using System.Collections.Generic;

namespace CkpServices.Interfaces
{
    public interface ISupplierService
    {
        List<SupplierInfoLight> GetSuppliers();
        SupplierInfoLight GetSupplier(int supplierId);
        List<RubricInfo> GetRubrics(int priceId);
        RubricInfo GetRubricVersion(int rubricId, DateTime rubricVersion);
        List<FormatTypeInfoLight> GetFormatTypes(int supplierId);
        FormatTypeInfoLight GetFormatType(int formatTypeId);
        IEnumerable<TariffInfo> GetTariffs(int supplierId, int formatTypeId);
        TariffInfo GetTariffVersion(int formatId, DateTime formatVersion, int priceId);
        List<GraphicInfo> GetGraphics(int supplierId, int formatTypeId);
        GraphicInfo GetGraphic(int graphicId);
        List<EducationInfo> GetEducations();
        List<ExperienceInfo> GetExperiences();
        List<CurrencyInfo> GetCurrencies();
        List<WorkGraphicInfo> GetWorkGraphics();
        List<OccurrenceInfo> GetOccurrencies(int supplierId);
    }
}
