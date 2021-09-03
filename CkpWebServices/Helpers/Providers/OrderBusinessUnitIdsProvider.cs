using CkpInfrastructure.Configuration;
using CkpInfrastructure.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CkpServices.Helpers.Providers
{
    public class OrderBusinessUnitIdsProvider : IProvider<int[]>
    {
        private readonly List<OrderSettings> _settings;

        public OrderBusinessUnitIdsProvider(List<OrderSettings> settings)
        {
            _settings = settings;
        }

        public int[] Get()
        {
            var businessUnitIds = _settings
                .Select(s => s.OrderBusinessUnitId)
                .Distinct()
                .ToArray();

            return businessUnitIds;
        }
    }
}
