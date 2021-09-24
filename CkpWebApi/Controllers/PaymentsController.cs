using CkpModel.Output;
using CkpServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;

        public PaymentsController(
            IHttpContextAccessor httpContextAccessor,
            IPaymentService paymentService)
        {
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
        }

        [HttpGet("balances/list")]
        public ActionResult<IEnumerable<BalanceInfo>> GetBalances()
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            var balances = _paymentService.GetBalance(clientLegalPersonId);

            return balances;
        }

        [HttpPut("pay-orders")]
        public IActionResult AutoPayOrders()
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            _paymentService.PayOrders(clientLegalPersonId);

            return Ok();
        }
    }
}
