using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CkpModel.Output;
using CkpServices.Interfaces;
using CkpServices.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        private readonly IOrderPositionService _orderPositionService;
        private readonly IUserService _userService;

        public AccountsController(
            IHttpContextAccessor httpContextAccessor,
            IAccountService accountService,
            IOrderPositionService orderPositionService,
            IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
            _orderPositionService = orderPositionService;
            _userService = userService;
        }

        [HttpGet("list/{startAccountId}/{quantity}")]
        public async Task<ActionResult<IEnumerable<AccountInfoLight>>> GetAccounts(int startAccountId, int quantity)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            var accounts = await _accountService.GetAccountsAsync(clientLegalPersonId, startAccountId, quantity);

            return accounts;
        }

        [HttpGet("item/{accountId}")]
        public async Task<ActionResult<AccountInfo>> GetAccount(int accountId)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            if (!_accountService.ExistsById(accountId))
                return StatusCode(
                    (int)HttpStatusCode.NotFound,
                    new { message = string.Format("Cчёт {0} не найден.", accountId) });

            if (!_userService.CanAccessAccount(clientLegalPersonId, accountId))
                return StatusCode(
                    (int)HttpStatusCode.Forbidden,
                    new { message = string.Format("Доступ к счёту {0} запрещён.", accountId) });

            var account = await _accountService.GetAccountByIdAsync(accountId);

            return account;
        }

        [HttpPost("item/create")]
        public IActionResult CreateAccount([FromBody] int[] orderPositionIds)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            if (orderPositionIds.Count() == 0)
                return StatusCode(
                    (int)HttpStatusCode.BadRequest, 
                    new { message = "Идентификаторы позиций счёта не переданы." });

            foreach (var orderPositionId in orderPositionIds)
            {
                if (!_orderPositionService.ExistsById(orderPositionId))
                    return StatusCode(
                        (int)HttpStatusCode.NotFound,
                        new { message = string.Format("Создание счёта. Позиция заказа {0} не найдена.", orderPositionId) });

                if (!_userService.CanAccessOrderPosition(clientLegalPersonId, orderPositionId))
                    return StatusCode(
                        (int)HttpStatusCode.Forbidden,
                        new { message = string.Format("Создание счёта. Доступ к позиции заказа {0} запрещён.", orderPositionId) }); ;
            }

            var accountId = _accountService.CreateClientAccount(orderPositionIds);

            return StatusCode((int)HttpStatusCode.OK, accountId);
        }

        [HttpGet("doc/{accountId}")]
        public async Task<IActionResult> GetAccountDocument(int accountId)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            if (!_accountService.ExistsById(accountId))
                return StatusCode(
                    (int)HttpStatusCode.NotFound,
                    new { message = string.Format("Получение документа. Cчёт {0} не найден.", accountId) });

            if (!_userService.CanAccessAccount(clientLegalPersonId, accountId))
                return StatusCode(
                    (int)HttpStatusCode.Forbidden,
                    new { message = string.Format("Получение документа. Доступ к счёту {0} запрещён.", accountId) });

            var account = await _accountService.GetFullAccountByIdAsync(accountId);

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
