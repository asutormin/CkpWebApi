using CkpWebApi.OutputEntities;
using CkpWebApi.OutputEntities.String;
using System;
using System.Collections.Generic;

namespace CkpWebApi.Services.Interfaces
{
    public interface ISupplierService
    {
        List<SupplierLight> GetSuppliers();
        SupplierLight GetSupplier(int supplierId);
        List<RubricInfo> GetRubrics(int priceId);
        RubricInfo GetRubricVersion(int rubricId, DateTime rubricVersion);
        List<FormatTypeLight> GetFormatTypes(int supplierId);
        FormatTypeLight GetFormatType(int formatTypeId);
        IEnumerable<TariffInfo> GetTariffs(int supplierId, int formatTypeId);
        TariffInfo GetTariffVersion(int formatId, DateTime formatVersion, int priceId);
        IEnumerable<TariffInfo> GetPackageTariffs(int supplierId, int formatTypeId, int formatId);
        List<GraphicInfo> GetGraphics(int supplierId, int formatTypeId);
        GraphicInfo GetGraphic(int graphicId);
        HandbookStorage GetHandbooks(int supplierId, int formatTypeId);
        List<Education> GetEducationsHandbook(int supplierId, int formatTypeId);
        List<Experience> GetExperiencesHandbook(int supplierId, int formatTypeId);
        List<Currency> GetCurrenciesHandbook(int supplierId, int formatTypeId);
        List<WorkGraphic> GetWorkGraphicsHandbook(int supplierId, int formatTypeId);
        List<Occurrence> GetOccurrenciesHandbook(int supplierId, int formatTypeId);
    }
}
