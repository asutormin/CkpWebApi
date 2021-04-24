using CkpEntities.Output;
using CkpEntities.Output.String;
using System;
using System.Collections.Generic;

namespace CkpServices.Interfaces
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
        List<GraphicInfo> GetGraphics(int supplierId, int formatTypeId);
        GraphicInfo GetGraphic(int graphicId);
        List<Education> GetEducationsHandbook(int supplierId, int formatTypeId);
        List<Experience> GetExperiencesHandbook(int supplierId, int formatTypeId);
        List<Currency> GetCurrenciesHandbook(int supplierId, int formatTypeId);
        List<WorkGraphic> GetWorkGraphicsHandbook(int supplierId, int formatTypeId);
        List<Occurrence> GetOccurrenciesHandbook(int supplierId, int formatTypeId);
    }
}
