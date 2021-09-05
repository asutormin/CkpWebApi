using CkpModel.Output.String;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CkpServices.Interfaces
{
    public interface IStringService
    {
        Task<List<AddressInfo>> GetAddressesAsync(int clientLegalPersonId, string description);
    }
}
