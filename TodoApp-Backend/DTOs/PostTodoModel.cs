using TodoApp_Backend.Models;

namespace TodoApp_Backend.DTOs
{
    public class PostTodoModel
    {
        public string title { get; set; } = string.Empty;
        public string? description { get; set; } = string.Empty;
        public string startDate { get; set; } = string.Empty;
        public string endDate { get; set; } = string.Empty;
        public string TodoPriority { get; set; } = string.Empty;
        public List<PostSubTodoModel>? SubTodos { get; set; } = new List<PostSubTodoModel>();
    }

    public class PostSubTodoModel
    {
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; }
    }
}
