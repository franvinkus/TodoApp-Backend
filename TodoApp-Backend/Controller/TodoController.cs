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
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
