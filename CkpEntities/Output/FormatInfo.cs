using System;

namespace CkpEntities.Output
{
    public class FormatInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float? FirstSize { get; set; }
        public float? SecondSize { get; set; }
        public bool EnableSecondSize { get; set; }
        public string UnitName { get; set; }
        public int PackageLength { get; set; }
        public string Description { get; set; }
        public DateTime Version { get; set; }
        public FormatTypeLight Type { get; set; }
    }
}
