using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp_Backend.DTOs;
using TodoApp_Backend.Services.Interface;

namespace TodoApp_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserServices _services;

        public UserController(UserServices services)
        {
            _services = services;
        }

        [HttpPost("user-register")]
        public async Task<IActionResult> Register([FromBody] UsersRegistrationRequest model, CancellationToken cancellationToken)
        {
            var result = await _services.Register(model, cancellationToken);

            if (result.Message.ToLower() == "success")
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("user-login")]
        public async Task<IActionResult> Login([FromBody] UsersLoginRequest model, CancellationToken cancellation)
        {
            var result = await _services.Login(model, cancellation);

            if (result.Message.ToLower() == "success")
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("jwt", result.Token, cookieOptions);

                return Ok(new {
                    Message = "Login sukses" ,
                    username = model.Username
                });
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("user-logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true, 
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.None

            });

            return Ok(new { Message = "Logout sukses" });

        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok("Server is awake and healthy!");
        }

        [HttpGet("cek-rahasia")]
        [Authorize] // <--- Gemboknya di sini!
        public IActionResult CekRahasia()
        {
            return Ok("Selamat! Kamu berhasil masuk ke area rahasia dengan Token.");
        }
    }
}