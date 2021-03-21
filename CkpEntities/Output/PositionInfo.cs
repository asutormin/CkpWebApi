using System.Collections.Generic;

namespace CkpEntities.Output
{
    public class PositionInfo
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public float ClientPrice { get; set; }
        public int Quantity { get; set; }
        public float ClientSum { get; set; }
        public float Nds { get; set; }
        public SupplierLight Supplier { get; set; }
        public FormatInfo Format { get; set; }
        public IEnumerable<RubricLight> Rubrics { get; set; }
        public IEnumerable<GraphicLight> Graphics { get; set; }
    }
}
