using CkpDAL;
using CkpInfrastructure.Providers.Interfaces;
using System.Linq;

namespace CkpServices.Helpers.Providers
{
    public class BusinessUnitIdByPriceIdProvider : IKeyedProvider<int, int>
    {
        private readonly BPFinanceContext _context;

        public BusinessUnitIdByPriceIdProvider(BPFinanceContext context)
        {
            _context = context;
        }

        public int GetByValue(int priceId)
        {
           return _context.Prices
                    .Single(p => p.Id == priceId)
                    .BusinessUnitId;
        }
    }
}
