using CkpInfrastructure.Configuration;
using CkpInfrastructure.Providers.Interfaces;
using System.Collections.Generic;

namespace CkpServices.Helpers.Providers
{
    public class OrderBusinessUnitIdProvider : IKeyedProvider<int, int>
    {
        private readonly IKeyedProvider<int, OrderSettings> _orderSettingsProvider;

        public OrderBusinessUnitIdProvider(List<OrderSettings> settings)
        {
            _orderSettingsProvider = new OrderSettingsProvider(settings);
        }

        public int GetByValue(int supplierId)
        {
            var orderSettings = _orderSettingsProvider.GetByValue(supplierId);

            return orderSettings.OrderBusinessUnitId;
        }
    }
}
