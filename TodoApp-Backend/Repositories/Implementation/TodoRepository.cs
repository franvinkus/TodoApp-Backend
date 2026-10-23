using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoApp_Backend.Data;
using TodoApp_Backend.Models;
using TodoApp_Backend.Repositories.Interface;

namespace TodoApp_Backend.Repositories.Implementation
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoAppDbContext _db;

        public TodoRepository(TodoAppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Todo>> GetRawTodoById(Guid userId, CancellationToken cancellationToken)
        {
            return await _db.Todos
                .AsNoTracking()
                .Include(x => x.Sub_Todos)
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Todo?> GetTodoById(int todoId, Guid userId, CancellationToken cancellationToken)
        {
            var todo = await _db.Todos
                .Include(x => x.Sub_Todos)
                .Where(x => x.Id == todoId && x.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            return todo;
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
