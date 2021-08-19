using System.Collections.Generic;

namespace CkpModel.Output
{
    public class SupplierInfo : SupplierInfoLight
    {
        public IEnumerable<FormatTypeInfo> FormatTypes { get; set; }

        public IEnumerable<RubricInfo> Rubrics { get; set; }
    }
}
