using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TodoApp_Backend.Data;
using TodoApp_Backend.Models;
using TodoApp_Backend.Repositories.Interface;

namespace TodoApp_Backend.Repositories.Implementation
{
    public class TodoRepository : ITodoRepository
    {
        private readonly IDistributedCache _d;
        private readonly TodoAppDbContext _db;

        public TodoRepository(IDistributedCache d, TodoAppDbContext db)
        {
            _d = d;
            _db = db;
        }

        public async Task<List<Todo>> GetRawTodoById(Guid userId, CancellationToken cancellationToken)
        {
            var cacheString = $"todos_{userId}";
            var getCached = await _d.GetStringAsync(cacheString, cancellationToken);

            if (!string.IsNullOrEmpty(getCached))
            {
                return JsonSerializer.Deserialize<List<Todo>>(getCached);
            }
            
            var rawTodos = await _db.Todos
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };

            await _d.SetStringAsync(cacheString, JsonSerializer.Serialize(rawTodos), cacheOptions);

            return rawTodos;
        }

        public async Task<Todo?> GetTodoById(int todoId, CancellationToken cancellationToken)
        {
            var todo = await _db.Todos
                .FirstOrDefaultAsync(x => x.Id == todoId, cancellationToken);

            return todo;
        }

        public async Task RemoveCache(Guid userId, CancellationToken cancellationToken)
        {
            var cacheString = $"todos_{userId}";
            await _d.RemoveAsync(cacheString, cancellationToken);
        }

        public void AddTodo(Todo todo)
        {
            _db.Todos.Add(todo);
        }

        public void RemoveTodo(Todo todo)
        {
            _db.Todos.Remove(todo);
        }

        public async Task SaveTodo(CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Todo>> GetPendingTodos(DateTime today, DateTime max, CancellationToken cancellationToken)
        {
            return await _db.Todos
                .Include(x => x.User)
                .Where(x => !x.IsFinished && x.EndDate.Date >= today && x.EndDate.Date <= max)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
