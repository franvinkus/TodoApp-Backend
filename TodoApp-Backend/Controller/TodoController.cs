using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp_Backend.DTOs;
using TodoApp_Backend.Services.Interface;

namespace TodoApp_Backend.Controller
{
    [Route("api/Todo")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        public readonly ITodoService _services;
        public TodoController(ITodoService services)
        {
            _services = services;
        }

        // GET: api/<TodoController>
        [HttpGet("Get")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] string? title, [FromQuery] string? sort, [FromQuery] string? prioritySort, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userIdString = Guid.Parse(userId);
            var data = await _services.GetTodo(title, sort, prioritySort, userIdString, cancellationToken);
            return Ok(data);
        }

        // POST api/<TodoController>
        [HttpPost("Post")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] PostTodoModel req, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Insert");
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userIdString = Guid.Parse(userId);
            var data = await _services.PostTodo(req, userIdString, cancellationToken);
            return Ok(data);
        }

        // PUT api/<TodoController>/5
        [HttpPut("PutTodo/{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, PutTodoModel edit, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Update");
            }

            var data = await _services.PutTodo(id, edit, cancellationToken);
            return Ok(data);

        }

        [HttpPut("PatchTodo/{id}")]
        [Authorize]
        public async Task<IActionResult> Patch(int id, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Update");
            }

            var data = await _services.PatchTodo(id, cancellationToken);
            return Ok(data);

        }

        // DELETE api/<TodoController>/5
        [HttpDelete("DeleteTodo/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Update");
            }

            var data = await _services.DeleteTodo(id, cancellationToken);
            return Ok(data);
        }
    }
}
