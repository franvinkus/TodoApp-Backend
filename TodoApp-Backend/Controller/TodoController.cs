using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp_Backend.Data;
using TodoApp_Backend.DTOs;
using TodoApp_Backend.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApp_Backend.Controller
{
    [Route("api/Todo")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        public readonly TodoAppDbContext _db;
        public readonly TodoServices _services;
        public TodoController(TodoAppDbContext db, TodoServices services)
        {
            _db = db;
            _services = services;
        }

        // GET: api/<TodoController>
        [HttpGet("Get")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] string? title, [FromQuery] string? sort)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userIdString = Guid.Parse(userId);
            var data = await _services.GetTodo(title, sort, userIdString);
            return Ok(data);
        }

        // POST api/<TodoController>
        [HttpPost("Post")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] PostTodoModel req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Insert");
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userIdString = Guid.Parse(userId);
            var data = await _services.PostTodo(req, userIdString);
            return Ok(data);
        }

        // PUT api/<TodoController>/5
        [HttpPut("PutTodo/{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, PutTodoModel edit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Update");
            }

            var data = await _services.PutTodo(id, edit);
            return Ok(data);

        }

        [HttpPut("PatchTodo/{id}")]
        [Authorize]
        public async Task<IActionResult> Patch(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Update");
            }

            var data = await _services.PatchTodo(id);
            return Ok(data);

        }

        // DELETE api/<TodoController>/5
        [HttpDelete("DeleteTodo/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Update");
            }

            var data = await _services.DeleteTodo(id);
            return Ok(data);
        }
    }
}
