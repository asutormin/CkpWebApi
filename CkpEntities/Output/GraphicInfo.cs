using System;

namespace CkpEntities.Output
{
    public class GraphicInfo : GraphicLight
    { 
        public DateTime DeliverDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public DateTime FinishDate { get; set; }
    }
}
