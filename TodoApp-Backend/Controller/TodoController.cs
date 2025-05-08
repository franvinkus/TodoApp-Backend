using Microsoft.AspNetCore.Mvc;
using Todo.Entities;
using TodoApp_Backend.Models;
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
        public async Task<IActionResult> Get(
            [FromQuery] string? title,
            [FromQuery] string? sort
            )
        {
            var data = await _services.GetTodo(title, sort);
            return Ok(data);
        }

        // POST api/<TodoController>
        [HttpPost("Post")]
        public async Task<IActionResult> Post([FromBody] PostTodoModel req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Insert");
            }

            var data = await _services.PostTodo(req);
            return Ok(data);
        }

        // PUT api/<TodoController>/5
        [HttpPut("PutTodo/{id}")]
        public async Task<IActionResult> Put(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Failed To Update");
            }

            var data = await _services.PutTodo(id);
            return Ok(data);

        }

        // DELETE api/<TodoController>/5
        [HttpDelete("DeleteTodo/{id}")]
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
