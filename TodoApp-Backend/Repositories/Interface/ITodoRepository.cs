using TodoApp_Backend.Models;

namespace TodoApp_Backend.Repositories.Interface
{
    public interface ITodoRepository
    {
        Task<List<Todo>> GetRawTodoById(Guid userId, CancellationToken cancellationToken);
        Task<Todo?> GetTodoById(int todoId, CancellationToken cancellationToken);
        Task RemoveCache(Guid userId, CancellationToken cancellationToken);
        void AddTodo(Todo todo);
        void RemoveTodo(Todo todo);
        Task SaveTodo(CancellationToken cancellationToken);
        Task<List<Todo>> GetPendingTodos(DateTime dateNow, DateTime max, CancellationToken cancellationToken);
    }
}
