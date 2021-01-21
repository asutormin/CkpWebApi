using CkpWebApi.InputEntities.Module;
using CkpWebApi.InputEntities.String;
using System.Collections.Generic;

namespace CkpWebApi.InputEntities
{
    public class Advertisement : AdvertisementLight
    {
        public AdvString String { get; set; }

        public AdvModule Module { get; set; }

        public List<Advertisement> Childs { get; set; }

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

            foreach (var graphic in Graphics)
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
