using CkpModel.Output.String;
using System.Collections.Generic;

namespace CkpModel.Output
{
    public class FormatTypeInfo : FormatTypeInfoLight
    {
        public IEnumerable<TariffInfo> Tariffs { get; set; }
        public IEnumerable<GraphicInfo> Graphics { get; set; }
        public HandbookStorageInfo Handbooks { get; set; }
    }
}
