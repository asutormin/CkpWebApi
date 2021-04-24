using CkpEntities.Input;
using CkpEntities.Output;
using CkpServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CkpWebApi.Controllers
{
    //[EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementsController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        [HttpGet("shoppingcart/{clientLegalPersonId}")]
        public async Task<ActionResult<IEnumerable<PositionInfo>>> GetShoppingCart(int clientLegalPersonId)
        {
            var shoppingCart = await _advertisementService.GetShoppingCartAsync(clientLegalPersonId);

            return shoppingCart;
        }

        [HttpGet("{orderPositionId}")]
        public Advertisement GetAdvertisement(int orderPositionId)
        {
            var advertisement = _advertisementService.GetAdvertisementFull(orderPositionId);

            return advertisement;
        }

        [HttpPost("create")]
        [DisableRequestSizeLimit]
        public IActionResult Create([FromBody] Advertisement advertisement)
        {
            if (advertisement == null)
                return BadRequest(new { message = "Создаваемая позиция не передана." });

            _advertisementService.CreateAdvertisement(advertisement);

            /*
            if (advertisement.Module != null)
            {
                var path = Path.Combine(Path.GetTempPath(), advertisement.Module.FileId);
                if (System.IO.File.Exists(path))
                    advertisement.Module.File = System.IO.File.ReadAllBytes(path);               
            }
            
            _advertisementService.CreateAdvertisement(advertisement);
            */

            /*
            if (aFile == null)
                return BadRequest(new { message = "Создаваемая позиция не передана." });

            string advContent = null;
            using (var reader = new StreamReader(aFile.OpenReadStream()))
            {
                advContent = reader.ReadToEnd();
            }

            var microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            var advertisement = JsonConvert.DeserializeObject<Advertisement>(advContent, microsoftDateFormatSettings);

            if (advertisement.Module != null)
            {
                var path = Path.Combine(Path.GetTempPath(), advertisement.Module.FileId);
                if (System.IO.File.Exists(path))
                    advertisement.Module.File = System.IO.File.ReadAllBytes(path);
            }

            _advertisementService.CreateAdvertisement(advertisement);
          */
            return Ok();
        }

        [HttpPut("update")]
        [DisableRequestSizeLimit]
        public IActionResult Update([FromBody] Advertisement advertisement)
        {
            if (advertisement == null)
                return BadRequest(new { message = "Изменяемая позиция не передана." });

            _advertisementService.UpdateAdvertisement(advertisement);

            return Ok();
        }

        [HttpDelete("delete/{query}")]
        public IActionResult Delete(string query)
        {
            if (query == null)
                return BadRequest(new { message = "Удаляемые позиции не переданы." });

            var regex = new Regex("id=([0-9]+)");
            var matches = regex.Matches(query);

            foreach (Match match in matches)
            {
                var orderPositionId = int.Parse(match.Groups[1].Value);
                _advertisementService.DeleteAdvertisement(orderPositionId);
            }

            return Ok();
        }




    }
}
