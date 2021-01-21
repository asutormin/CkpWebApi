using System;

namespace CkpWebApi.OutputEntities
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
