using CkpWebApi.OutputEntities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CkpWebApi.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ActionResult<IEnumerable<AccountLight>>> GetAccountsAsync(int clientLegalPersonId, int startAccountId, int quantity);
        Task<ActionResult<AccountInfo>> GetAccountAsync(int accountId);
    }
}
