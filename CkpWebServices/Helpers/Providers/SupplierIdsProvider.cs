using CkpInfrastructure.Configuration;
using CkpInfrastructure.Providers.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices.Helpers.Providers
{
    public class SupplierIdsProvider : IProvider<int[]>
    {
        private readonly List<OrderSettings> _settings;
        
        public SupplierIdsProvider(List<OrderSettings> settings)
        {
            _settings = settings;
        }

        public int[] Get()
        {
            var supplierIds = _settings
                .Where(s => s.SupplierId > 0)
                .Select(s => s.SupplierId)
                .ToArray();

            return supplierIds;
        }
    }
}
