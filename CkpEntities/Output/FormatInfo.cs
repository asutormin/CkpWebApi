using System;

namespace CkpEntities.Output
{
    public class FormatInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float? FirstSize { get; set; }
        public float? SecondSize { get; set; }
        public int PackageLength { get; set; }
        public DateTime Version { get; set; }
        public FormatTypeLight Type { get; set; }
    }
}
