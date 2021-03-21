using CkpEntities.Output.String;
using System.Collections.Generic;

namespace CkpEntities.Output
{
    public class FormatTypeInfo : FormatTypeLight
    {
        public IEnumerable<TariffInfo> Tariffs { get; set; }
        public IEnumerable<GraphicInfo> Graphics { get; set; }
        public HandbookStorage Handbooks { get; set; }
    }
}
