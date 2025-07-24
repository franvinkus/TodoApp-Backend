namespace TodoApp_Backend.Models
{
    public class PutTodoModel
    {   
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

    }
}
