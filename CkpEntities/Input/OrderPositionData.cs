using CkpModel.Input.Module;
using CkpModel.Input.String;
using System.Collections.Generic;

namespace CkpModel.Input
{
    public class OrderPositionData : OrderPositionDataLight
    {
        public StringData StringData { get; set; }

        public ModuleData ModuleData { get; set; }

        public List<OrderPositionData> Childs { get; set; }

        public List<int> GetPrices()
        {
            var prices = new List<int>();

            prices.Add(PriceId);

            foreach (var child in Childs)
                prices.Add(child.PriceId);

            return prices;
        }

        public List<int> GetGraphicsWithChildren()
        {
            var graphics = new List<int>();

            foreach (var graphic in GraphicsData)
            {
                graphics.Add(graphic.Id);
                graphics.AddRange(graphic.Childs);
            }

            foreach (var child in Childs)
                graphics.AddRange(child.GetGraphicsWithChildren());

            return graphics;
        }
    }
}
