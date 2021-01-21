using System.Collections.Generic;

namespace CkpWebApi.OutputEntities
{
    public class SupplierInfo : SupplierLight
    {
        public IEnumerable<FormatTypeInfo> FormatTypes { get; set; }

        public IEnumerable<RubricInfo> Rubrics { get; set; }
    }
}
