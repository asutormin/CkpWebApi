using System.Collections.Generic;

namespace CkpEntities.Output
{
    public class SupplierInfo : SupplierLight
    {
        public IEnumerable<FormatTypeInfo> FormatTypes { get; set; }

        public IEnumerable<RubricInfo> Rubrics { get; set; }
    }
}
