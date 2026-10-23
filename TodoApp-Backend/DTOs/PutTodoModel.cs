using TodoApp_Backend.Models;

namespace TodoApp_Backend.DTOs
{
    public class PutTodoModel
    {   
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string startDate { get; set; } = string.Empty;
        public string endDate { get; set; } = string.Empty;
        public string TodoPriority { get; set; } = string.Empty;
        public List<PutSubTodoModel>? SubTodos { get; set; } = new List<PutSubTodoModel>();
    }

    public class PutSubTodoModel
    {
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; }
    }
}
