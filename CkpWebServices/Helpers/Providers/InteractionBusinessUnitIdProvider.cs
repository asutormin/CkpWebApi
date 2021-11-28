using CkpDAL;
using CkpInfrastructure.Providers.Interfaces;
using System.Linq;

namespace CkpServices.Helpers.Providers
{
    public class InteractionBusinessUnitIdProvider : IKeyedProvider<int, int>
    {
        private readonly BPFinanceContext _context;

        public InteractionBusinessUnitIdProvider(BPFinanceContext context)
        {
            _context = context;
        }

        public int GetByValue(int clientLegalPersonId)
        {
            return _context.AccountSettings
                .Single(acs => acs.LegalPersonId == clientLegalPersonId)
                .InteractionBusinessUnitId;
        }
    }
}
