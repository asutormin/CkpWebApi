using CkpInfrastructure.Providers.Interfaces;

namespace CkpServices.Helpers.Providers
{
    public class OrderImFilePathProvider : IKeyedProvider<int, string>
    {
        private readonly string _template;

        public OrderImFilePathProvider(string template)
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
