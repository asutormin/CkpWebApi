using CkpModel.Input;
using CkpServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthData authData)
        {
            var authInfo = _userService.Authenticate(authData);

            if (authInfo == null)
                return BadRequest(new { message = "Передан неверный логин или пароль." });

            return Ok(authInfo);
        }
    }
}
