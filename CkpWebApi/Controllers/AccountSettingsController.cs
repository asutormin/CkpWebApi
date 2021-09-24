using CkpServices.Processors.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [Route("api/account-settings")]
    [ApiController]
    public class AccountSettingsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountSettingsService _accountSettingsService;

        public AccountSettingsController(
            IHttpContextAccessor httpContextAccessor,
            IAccountSettingsService accountSettingsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountSettingsService = accountSettingsService;
        }

        [HttpGet("is-need-prepayment")]
        public ActionResult<bool> GetIsNeedPrepayment()
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            var isNeedPrepayment = _accountSettingsService.GetIsNeedPrepayment(clientLegalPersonId);

            return isNeedPrepayment;
        }
    }
}
