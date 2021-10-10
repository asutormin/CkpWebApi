using CkpInfrastructure.Configuration;
using CkpInfrastructure.Providers.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices.Helpers.Providers
{
    public class PaymentInTimeDiscountProvider : IKeyedProvider<int, float>
    {
        private readonly List<BusinessUnitSettings> _businessUnitSettings;

        public PaymentInTimeDiscountProvider(List<BusinessUnitSettings> businessUnitSettings)
        {
            _businessUnitSettings = businessUnitSettings;
        }

        public float GetByValue(int businessUnitId)
        {
            var settings = _businessUnitSettings
                .SingleOrDefault(settings => settings.BusinessUnitId == businessUnitId);

            if (settings == null) return 0;
                       
            return settings.PaymentInTimeDiscountPercent;
        }
    }
}
