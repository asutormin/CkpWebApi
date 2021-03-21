using System.Collections.Generic;

namespace CkpEntities.Output
{
    public class TariffInfo
    {
        public SupplierLight Supplier { get; set; }
        public FormatInfo Format { get; set; }
        public PriceInfo Price { get; set; }
        public List<TariffInfo> PackageTariffs { get; set; }
    }
}
