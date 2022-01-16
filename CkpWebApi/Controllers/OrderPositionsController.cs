using CkpModel.Input;
using CkpModel.Output;
using CkpServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [Route("api/order-positions")]
    [ApiController]
    public class OrderPositionsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderPositionService _orderPositionService;
        private readonly IUserService _userService;

        public OrderPositionsController(
            IHttpContextAccessor httpContextAccessor,
            IOrderPositionService orderPositionService,
            IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _orderPositionService = orderPositionService;
            _userService = userService;
        }

        [HttpGet("basket")]
        public IEnumerable<OrderPositionInfo> GetBasket()
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            var basket = _orderPositionService.GetBasket(clientLegalPersonId);

            return basket;
        }

        [HttpGet("item/{orderPositionId}")]
        public async Task<ActionResult<OrderPositionData>> GetOrderPosition(int orderPositionId)
        {
            var auth = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            if (!_orderPositionService.ExistsById(orderPositionId))
                return StatusCode(
                    (int)HttpStatusCode.NotFound,
                    new { message = string.Format("Запрашиваемая позиция заказа {0} не найдена.", orderPositionId) });

            if (!_userService.CanAccessOrderPosition(auth, orderPositionId))
                return StatusCode(
                    (int)HttpStatusCode.Forbidden,
                    new { message = string.Format("Доступ к позиции заказа {0} запрещён.", orderPositionId) });

            var orderPosition = await _orderPositionService.GetOrderPositionDataAsync(orderPositionId);

            return orderPosition;
        }

        [HttpPost("item/create")]
        [DisableRequestSizeLimit]
        public IActionResult Create([FromBody] OrderPositionData orderPosition)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            if (orderPosition == null)
                return BadRequest(
                    new { message = "Создаваемая позиция не передана." });

            if (clientLegalPersonId != orderPosition.ClientLegalPersonId)
                return StatusCode(
                    (int)HttpStatusCode.Forbidden,
                    new { message = "Создание позиции заказа запрещено." });

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(JsonSerializer.Serialize(orderPosition));

            if (orderPosition.StringData != null && !orderPosition.StringData.PhonesData.Any())
                throw new Exception("В строку не переданы телефоны.");

            _orderPositionService.CreateOrderPosition(orderPosition);

            return Ok();
        }

        [HttpPut("item/update")]
        [DisableRequestSizeLimit]
        public IActionResult Update([FromBody] OrderPositionData orderPosition)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            if (orderPosition == null)
                return BadRequest(
                    new { message = "Изменяемая позиция не передана." });

            if (!_orderPositionService.ExistsById(orderPosition.OrderPositionId))
                return StatusCode(
                    (int)HttpStatusCode.NotFound,
                    new { message = string.Format("Изменяемая позиция заказа {0} не найдена.", orderPosition.OrderPositionId) });

            if (clientLegalPersonId != orderPosition.ClientLegalPersonId)
                return StatusCode(
                    (int)HttpStatusCode.Forbidden,
                    new { message = string.Format("Изменение позиции заказа {0} запрещено.", orderPosition.OrderPositionId) });

            _orderPositionService.UpdateOrderPosition(orderPosition);

            return Ok();
        }

        [HttpDelete("item/delete/{orderPositionIdsQuery}")]
        public IActionResult Delete(string orderPositionIdsQuery)
        {
            var clientLegalPersonId = _httpContextAccessor.HttpContext.GetClientLegalPersonId();

            if (orderPositionIdsQuery == null)
                return BadRequest(
                    new { message = "Удаляемые позиции не переданы." });

            var regex = new Regex("id=([0-9]+)");
            var matches = regex.Matches(orderPositionIdsQuery);

            var orderPositionIds = matches
                .OfType<Match>()
                .Select(m => int.Parse(m.Groups[1].Value));

            foreach (var orderPositionId in orderPositionIds)
            {
                if (!_orderPositionService.ExistsById(orderPositionId))
                    return StatusCode(
                        (int)HttpStatusCode.NotFound,
                        new { message = string.Format("Удаляемая позиция заказа {0} не найдена.", orderPositionId) });

                if (!_userService.CanAccessOrderPosition(clientLegalPersonId, orderPositionId))
                    return StatusCode(
                        (int)HttpStatusCode.Forbidden,
                        new { message = string.Format("Удаление позиции заказа {0} запрещено.", orderPositionId) }); ;
            }

            foreach (var orderPositionId in orderPositionIds)
                _orderPositionService.DeleteOrderPosition(orderPositionId);

            return Ok();
        }
    }
}
