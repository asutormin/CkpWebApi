using CkpWebApi.Infrastructure.Providers.Interfaces;
using System.Linq;

namespace CkpWebApi.Helpers.Providers
{
    public class OrderImFileNameProvider : IKeyedProvider<int, string>
    {
        private readonly string _template;

        public OrderImFileNameProvider(string template)
        {
            _template = template;
        }

        public string GetByValue(int orderPositionId)
        {
            var orderPositionIdString = orderPositionId.ToString();

            return string.Format(_template, orderPositionIdString.Substring(0, 3), orderPositionIdString);
        }
    }
}
