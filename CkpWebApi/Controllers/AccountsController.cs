using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CkpEntities.Output;
using CkpServices.Interfaces;
using CkpServices.Helpers;
using System.Linq;

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
            var account = await _accountService.GetAccountByIdAsync(accountId);

            return account;

            //return Ok();
        }

        [HttpGet("{clientLegalPersonId}/{startAccountId}/{quantity}")]
        public async Task<ActionResult<IEnumerable<AccountLight>>> GetAccounts(int clientLegalPersonId, int startAccountId, int quantity)
        {
            var accounts = await _accountService.GetAccountsAsync(clientLegalPersonId, startAccountId, quantity);

            return accounts;
        }

        [HttpPost("create")]
        public IActionResult CreateAccount([FromBody] int[] orderPositionIds)
        {
            if (orderPositionIds.Count() == 0)
                return BadRequest(new { message = "Идентификаторы позиций счёта не переданы." });

            var accountId = _accountService.CreateClientAccount(orderPositionIds);

            return Ok(accountId);
        }

        [HttpGet("document/{accountId}")]
        public async Task<IActionResult> GetAccountDocument(int accountId)
        {
            var account = await _accountService.GetAccountById(accountId);

            if (account == null)
                return BadRequest(new { message = "Счёт не найден." });

            var exporter = new AccountDocumentExporter
            {
                FullDetails = true,
                WithStamp = true,
                WithStampDate = false,
                Account = account.Value
            };

            var bytes = exporter.Export();

            return File(bytes, "application/vnd.openxmlformats", string.Format("Счёт (с печатью)_№{0}.docx", account.Value.Number));
        }
    }
}
