using CkpInfrastructure.Configuration;
using CkpInfrastructure.Providers.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices.Helpers.Providers
{
    public class OrderSettingsProvider : IKeyedProvider<int, OrderSettings>
    {
        private readonly List<OrderSettings> _settings;
        public OrderSettingsProvider(List<OrderSettings> settings)
        {
            _settings = settings;
        }

        public OrderSettings GetByValue(int supplierId)
        {
            var orderSettings = _settings
                .FirstOrDefault(s => s.SupplierId == supplierId);

            if (orderSettings == null)
                orderSettings = _settings.First(s => s.SupplierId == 0);

            return orderSettings;
        }
    }
}
