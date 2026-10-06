using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TodoApp_Backend.Constant;
using TodoApp_Backend.Data;
using TodoApp_Backend.DTOs;
using TodoApp_Backend.Models;

namespace TodoApp_Backend.Services
{
    public class TodoServices
    {
        private readonly TodoAppDbContext _db;
        private readonly IDistributedCache _c;

        public TodoServices(TodoAppDbContext db, IDistributedCache c)
        {
             _db = db;
             _c = c;
        }

        public async Task<List<GetTodoModel>> GetTodo(string? title, string? sort, string? prioritySort, Guid userId)
        {
            var cacheString = $"todos_{userId}";
            List<Todo> rawTodo;

            var getCached = await _c.GetStringAsync(cacheString);

            if (!string.IsNullOrEmpty(getCached))
            {
                rawTodo = JsonSerializer.Deserialize<List<Todo>>(getCached);
            }
            else
            {
                rawTodo = await _db.Todos.Where(x => x.UserId == userId).ToListAsync();

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                };

                await _c.SetStringAsync(cacheString, JsonSerializer.Serialize(rawTodo), cacheOptions);
            }

            var query = rawTodo.AsEnumerable();

            query = query.Where(x => x.UserId == userId);

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(Q => Q.Title.ToLower().Contains(title.ToLower().Trim()));
            }

            IOrderedEnumerable<Todo> orderedQuery;

            if (!string.IsNullOrWhiteSpace(prioritySort))
            {
                orderedQuery = prioritySort.ToLower() switch
                {
                    "high" => query.OrderByDescending(t => t.TodoPriority),
                    "low" => query.OrderBy(t => t.TodoPriority),
                    _ => query.OrderByDescending(t => t.TodoPriority),
                };

                orderedQuery = (string.IsNullOrWhiteSpace(sort) ? "latest" : sort.ToLower()) switch
                {
                    "oldest" => orderedQuery.ThenBy(t => t.CreatedDate),
                    "latest" => orderedQuery.ThenByDescending(t => t.CreatedDate),
                    _ => orderedQuery.ThenByDescending(t => t.CreatedDate)
                };
            }
            else
            {
                orderedQuery = (string.IsNullOrWhiteSpace(sort) ? "latest" : sort.ToLower()) switch
                {
                    "oldest" => query.OrderBy(t => t.CreatedDate),
                    "latest" => query.OrderByDescending(t => t.CreatedDate),
                    _ => query.OrderByDescending(t => t.CreatedDate)
                };
            }

            return orderedQuery.Select(t => new GetTodoModel
            {
                id = t.Id,
                title = t.Title,
                description = t.Description,
                createdAt = t.CreatedDate.ToString("dd-MM-yyyy HH:mm:ss"),
                finishedAt = t.FinishedDate.HasValue ? t.FinishedDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : "-",
                startDate = t.StartDate.ToString("dd-MM-yyyy HH:mm:ss"),
                endDate = t.EndDate.ToString("dd-MM-yyyy HH:mm:ss"),
                isCompleted = t.IsFinished,
                TodoPriority = t.TodoPriority.ToString()
            }).ToList();
        }

        public async Task<string> PostTodo(PostTodoModel req, Guid userId)
        {
            var newData = new Models.Todo
            {
                Title = req.title,
                Description = req.description,
                CreatedDate = DateTime.UtcNow,
                StartDate = DateTime.SpecifyKind(Convert.ToDateTime(req.startDate), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(Convert.ToDateTime(req.endDate), DateTimeKind.Utc),
                IsFinished = false,
                TodoPriority = Enum.Parse<PriorityEnum>(req.TodoPriority, true),
                UserId = userId,
            };

            _db.Todos.Add(newData);
            await _db.SaveChangesAsync();
            await _c.RemoveAsync($"todos_{userId}");

            return "Success";
        }

        public async Task<string> PutTodo(int id, PutTodoModel edit)
        {
            var isIdExist = await _db.Todos
                .Where(Q => Q.Id == id)
                .FirstOrDefaultAsync();

            if(isIdExist == null)
            {
                return "Id not Found";
            }

            isIdExist.Title = edit.Title;
            isIdExist.Description = edit.Description;
            isIdExist.StartDate = DateTime.SpecifyKind(Convert.ToDateTime(edit.startDate), DateTimeKind.Utc);
            isIdExist.EndDate = DateTime.SpecifyKind(Convert.ToDateTime(edit.endDate), DateTimeKind.Utc);
            isIdExist.TodoPriority = Enum.Parse<PriorityEnum>(edit.TodoPriority, true);

            await _db.SaveChangesAsync();
            await _c.RemoveAsync($"todos_{isIdExist.UserId}");

            return "Success";
        }

        public async Task<string> PatchTodo(int id)
        {
            var isIdExist = await _db.Todos
                .Where(Q => Q.Id == id)
                .FirstOrDefaultAsync();

            if (isIdExist == null)
            {
                return "Id not Found";
            }

            isIdExist.IsFinished = !isIdExist.IsFinished;

            if (!isIdExist.IsFinished)
            {
                isIdExist.FinishedDate = null;
            }
            else
            {
                isIdExist.FinishedDate = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            await _c.RemoveAsync($"todos_{isIdExist.UserId}");

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
            await _c.RemoveAsync($"todos_{isIdExist.UserId}");

            return "Success";
        }
    }
}
