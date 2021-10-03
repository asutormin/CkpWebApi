using CkpModel.Output;
using CkpServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISearchService _searchService;

        public SearchController(
            IHttpContextAccessor httpContextAccessor,
            ISearchService searchService)
        {
            _httpContextAccessor = httpContextAccessor;
            _searchService = searchService;
        }

        [HttpGet("list")]
        public ActionResult<IEnumerable<OrderPositionInfo>> Search(string value, int skipCount)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            var orderPositions = _searchService.Search(clientLegalPersonId, value, skipCount);

            return orderPositions;
        }
    }
}
