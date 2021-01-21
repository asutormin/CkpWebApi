using CkpWebApi.DAL.Model;
using CkpWebApi.Helpers.FileNameProviders;
using CkpWebApi.Infrastructure.Providers;
using CkpWebApi.Infrastructure.Providers.Interfaces;
using System.Linq;

namespace CkpWebApi.Helpers
{
    public class VerstkaFileNameBuilder
    {
        public IKeyedProvider<string, string> ClientNameProvider = new FunctionBasedKeyedProvider<string, string>(name => name);
        public IKeyedProvider<string, string> PricePositionNameProvider = new FunctionBasedKeyedProvider<string, string>(name => name);
        public IKeyedProvider<string, string> RubricNumberProvider = new FunctionBasedKeyedProvider<string, string>(number => number);
        public IKeyedProvider<int, string> VerstkaIdProvider = new FunctionBasedKeyedProvider<int, string>((id) => id.ToString());
                
        public string Build(OrderPosition orderPosition)
        {
            var clientName = ClientNameProvider.GetByValue(orderPosition.Order.ClientCompany.Name);
            var pricePositionName = PricePositionNameProvider.GetByValue(orderPosition.PricePosition.Name);
            var rubricNumber = RubricNumberProvider.GetByValue(orderPosition.RubricPositions.First().Rubric.Number);
            var positionImIdString = VerstkaIdProvider.GetByValue(orderPosition.Id);

            var fileName = string.Format("{0}{1}{2}-{3}",     //<клиент><формат><рубрика>-<id>
                clientName,
                pricePositionName,
                rubricNumber,
                positionImIdString);

            return fileName;
        }

    }
}
