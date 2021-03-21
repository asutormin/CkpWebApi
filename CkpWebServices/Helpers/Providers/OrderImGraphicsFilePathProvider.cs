using CkpInfrastructure.Providers.Interfaces;

namespace CkpServices.Helpers.Providers
{
    public class OrderImGraphicsFilePathProvider : IKeyedProvider<string, string>
    {
        private readonly string _template;

        public OrderImGraphicsFilePathProvider(string template)
        {
            _template = template;
        }

        public string GetByValue(string fileName)
        {
            return _template + "\\" + fileName;
        }
    }
}
