using System;

namespace CkpModel.Output
{
    public class GraphicInfo : GraphicInfoLight
    { 
        public DateTime DeliverDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public DateTime FinishDate { get; set; }
    }
}
