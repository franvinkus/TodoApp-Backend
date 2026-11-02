using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp_Backend.DTOs;
using TodoApp_Backend.Models;
using TodoApp_Backend.Services.Interface;

namespace TodoApp_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _services;

        public UserController(IUserService services)
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
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

                return Ok(new {
                    Message = "Login sukses" ,
                    username = model.Username,
                    token = result.Token
                });
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("user-logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if(Request.Cookies.TryGetValue("refreshToken", out var Token))
            {
                await _services.Logout(Token, cancellationToken);
            }

            Response.Cookies.Delete("refreshToken");

            return Ok(new { Message = "Logout sukses" });

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            if(!Request.Cookies.TryGetValue("refreshToken", out var oldRefreshToken))
            {
                return Unauthorized(new { Message = "No Refresh Token Provided." });
            }

            var result = await _services.RefreshToken(oldRefreshToken, cancellationToken);

            if (result.Message.ToLower() != "success")
            {
                return Unauthorized(new { Message = result.Message });
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

            return Ok(new
            {
                token = result.Token
            });
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