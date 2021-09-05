using CkpDAL;
using CkpDAL.Repository;
using CkpInfrastructure.Configuration;
using CkpModel.Output.String;
using CkpServices.Interfaces;
using CkpServices.Processors.Interfaces;
using CkpServices.Processors.String;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CkpServices
{
    public class StringService : IStringService
    {
        private readonly BPFinanceContext _context;
        private readonly IStringProcessor _stringProcessor;

        public StringService(BPFinanceContext context, IOptions<AppParams> appParamsAccessor)
        {
            _context = context;
            var repository = new BPFinanceRepository(_context, appParamsAccessor.Value.EditUserId);

            _stringProcessor = new StringProcessor(
                _context,
                repository);
        }

        public async Task<List<AddressInfo>> GetAddressesAsync(int clientLegalPersonId, string description)
        {
            return await _stringProcessor.GetAddressesAsync(clientLegalPersonId, description);
        }
    }
}
