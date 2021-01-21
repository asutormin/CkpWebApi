using CkpWebApi.OutputEntities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CkpWebApi.Services.Interfaces;

namespace CkpWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{accountId}")]
        public async Task<ActionResult<AccountInfo>> GetAccount(int accountId)
        {
            //_accountService.AccountTest(accountId);
            var account = await _accountService.GetAccountAsync(accountId);

            return account;

            //return Ok();
        }

        [HttpGet("{clientLegalPersonId}/{startAccountId}/{quantity}")]
        public async Task<ActionResult<IEnumerable<AccountLight>>> GetAccounts(int clientLegalPersonId, int startAccountId, int quantity)
        {
            var accounts = await _accountService.GetAccountsAsync(clientLegalPersonId, startAccountId, quantity);

            return accounts;
        }
    }
}
