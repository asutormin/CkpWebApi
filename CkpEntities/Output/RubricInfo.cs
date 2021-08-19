using System;

namespace CkpModel.Output
{
    public class RubricInfo : RubricInfoLight
    {
        public int? ParentId { get; set; }

        public int SupplierId { get; set; }

        public int OrderBy { get; set; }

        public bool CanUse { get; set; }

        public DateTime Version { get; set; }
    }
}
