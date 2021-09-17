using System.Collections.Generic;

namespace CkpModel.Output
{
    public class OrderPositionInfo
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public float ClientCost { get; set; }
        public int Quantity { get; set; }
        public float ClientSum { get; set; }
        public float Nds { get; set; }
        public SupplierInfoLight Supplier { get; set; }
        public FormatInfo Format { get; set; }
        public PriceInfo Price { get; set; }
        public IEnumerable<RubricInfoLight> Rubrics { get; set; }
        public IEnumerable<GraphicInfoLight> Graphics { get; set; }
    }
}
