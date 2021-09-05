using System.Collections.Generic;
using System.Threading.Tasks;
using CkpModel.Output.String;
using CkpServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringService _stringService;

        public AddressesController(
            IHttpContextAccessor httpContextAccessor,
            IStringService stringService)
        {
            _httpContextAccessor = httpContextAccessor;
            _stringService = stringService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<AddressInfo>>> GetAddresses(string description)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            return await _stringService.GetAddressesAsync(clientLegalPersonId, description);
        }

    }
}
