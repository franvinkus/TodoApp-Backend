using Microsoft.EntityFrameworkCore;
using Todo.Entities;
using TodoApp_Backend.Models;

namespace TodoApp_Backend.Services
{
    public class TodoServices
    {
        public readonly TodoAppDbContext _db;
        public TodoServices(TodoAppDbContext db)
        {
             _db = db;
        }

        public async Task<List<GetTodoModel>> GetTodo(string? title, string sort)
        {
            var query = _db.Todos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(Q => Q.Title.ToLower().Contains(title.ToLower().Trim()));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort.ToLower() switch
                {
                    "oldest" => query.OrderBy(t => t.CreatedDate),
                    "latest" => query.OrderByDescending(t => t.CreatedDate),
                    _ => query
                };
            }

            var todos = await query.ToListAsync();

            return todos.Select(t => new GetTodoModel
            {
                id = t.Id,
                title = t.Title,
                description = t.Description,
                createdAt = t.CreatedDate.ToString("dd-MM-yyyy HH:mm:ss"),
                finishedAt = t.FinishedDate.HasValue ? t.FinishedDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : "-",
                isCompleted = t.IsFinished
            }).ToList();
        }

        public async Task<string> PostTodo(PostTodoModel req)
        {
            var newData = new Todo.Entities.Todo
            {
                Title = req.title,
                Description = req.description,
                CreatedDate = DateTime.Now,
                IsFinished = false,
            };

            _db.Todos.Add(newData);
            await _db.SaveChangesAsync();

            return "Success";
        }

        public async Task<string> PutTodo(int id)
        {
            var isIdExist = await _db.Todos
                .Where(Q => Q.Id == id)
                .FirstOrDefaultAsync();

            if(isIdExist == null)
            {
                return "Id not Found";
            }

            isIdExist.IsFinished = !isIdExist.IsFinished;

            await _db.SaveChangesAsync();

            return "Success";
        }

        public async Task<string> DeleteTodo(int id)
        {
            var isIdExist = await _db.Todos
                .Where(Q => Q.Id == id)
                .FirstOrDefaultAsync();

            if (isIdExist == null)
            {
                return "Id not Found";
            }

            _db.Todos.Remove(isIdExist);

            await _db.SaveChangesAsync();

            return "Success";
        }
    }
}
