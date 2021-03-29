using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CkpDAL.Model;
using CkpEntities.Output;

namespace CkpServices.Interfaces
{
    public interface IAccountService
    {
        Task<ActionResult<IEnumerable<AccountLight>>> GetAccountsAsync(int clientLegalPersonId, int startAccountId, int quantity);
        Task<ActionResult<AccountInfo>> GetAccountByIdAsync(int accountId);
        Task<ActionResult<Account>> GetAccountById(int accountId);
        int CreateClientAccount(int[] orderPositionIds);
    }
}
