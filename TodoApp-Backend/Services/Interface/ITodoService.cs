using TodoApp_Backend.DTOs;

namespace TodoApp_Backend.Services.Interface
{
    public interface ITodoService
    {
        Task<List<GetTodoModel>> GetTodo(string? title, string? sort, string? prioritySort, Guid userId, CancellationToken cancellationToken);
        Task<string> PostTodo(PostTodoModel req, Guid userId, CancellationToken cancellationToken);
        Task<string> PutTodo(int id, PutTodoModel edit, CancellationToken cancellationToken);
        Task<string> PatchTodo(int id, CancellationToken cancellationToken);
        Task<string> DeleteTodo(int id, CancellationToken cancellationToken);

    }
}
