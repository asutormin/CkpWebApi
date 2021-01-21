using CkpWebApi.OutputEntities.String;
using System.Collections.Generic;

namespace CkpWebApi.OutputEntities
{
    public class FormatTypeInfo : FormatTypeLight
    {
        public IEnumerable<TariffInfo> Tariffs { get; set; }
        public IEnumerable<GraphicInfo> Graphics { get; set; }
        public HandbookStorage Handbooks { get; set; }
    }
}
