using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CkpDAL.Entities;
using CkpModel.Output;

namespace CkpServices.Interfaces
{
    public interface IAccountService
    {
        bool ExistsById(int accountId);
        Task<ActionResult<IEnumerable<AccountInfoLight>>> GetAccountsAsync(int clientLegalPersonId, int startAccountId, int quantity);
        Task<ActionResult<AccountInfo>> GetAccountByIdAsync(int accountId);
        Task<ActionResult<Account>> GetFullAccountByIdAsync(int accountId);
        int CreateClientAccount(int[] orderPositionIds);
    }
}
