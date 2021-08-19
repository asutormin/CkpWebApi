using System.Collections.Generic;

namespace CkpModel.Output
{
    public class TariffInfo
    {
        public SupplierInfoLight Supplier { get; set; }
        public FormatInfo Format { get; set; }
        public PriceInfo Price { get; set; }
        public List<TariffInfo> PackageTariffs { get; set; }
    }
}
