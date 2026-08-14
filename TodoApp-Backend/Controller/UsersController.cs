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
                return Ok(result.Token);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("cek-rahasia")]
        [Authorize] // <--- Gemboknya di sini!
        public IActionResult CekRahasia()
        {
            return Ok("Selamat! Kamu berhasil masuk ke area rahasia dengan Token.");
        }
    }
}