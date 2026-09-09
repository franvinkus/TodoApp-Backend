using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using TodoApp_Backend.Services;

namespace TodoApp_Backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly ReminderServices _s;
        private readonly IConfiguration _c;

        public ReminderController(ReminderServices s, IConfiguration c)
        {
            _s = s;
            _c = c;
        }

        [HttpPost("reminder-job")]
        public async Task<IActionResult> GetReminderJob([FromHeader(Name = "X-Job-Secret")] string secret)
        {
            var expectedSecret = _c["JobSecret"];
            if (string.IsNullOrEmpty(secret) || secret != expectedSecret)
            {
                return Unauthorized(new { Message = "Akses ditolak." });
            }

            try
            {
                await _s.SendDailyReminder();
                return Ok(new { Message = "Reminder job executed successfully." });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing reminder job: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while executing the reminder job." });
            }
        }
    }
}
