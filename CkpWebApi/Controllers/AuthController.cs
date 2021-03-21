using CkpEntities.Input;
using CkpServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CkpWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] LoginInfo loginInfo)
        {
            var authInfo = _authService.Authenticate(loginInfo.Login, loginInfo.Password, true);

            if (authInfo == null)
                return BadRequest(new { message = "Передан неверный логин или пароль." });

            return Ok(authInfo);
        }

        [HttpPatch("login/set/{oldLogin}/{newLogin}")]
        public IActionResult SetLogin(string oldLogin, string newLogin)
        {
            var authInfo = _authService.SetLogin(oldLogin, newLogin);

            if (authInfo == null)
                return BadRequest(new { message = "Передан неверный логин." });

            return Ok(authInfo);
        }

        [HttpPatch("password/set/{login}/{newPassword}")]
        public IActionResult SetPassword(string login, string newPassword)
        {
            var authInfo = _authService.SetPassword(login, newPassword);

            if (authInfo == null)
                return BadRequest(new { message = "Передан неверный логин или пароль." });

            return Ok(authInfo);
        }
    }
}
