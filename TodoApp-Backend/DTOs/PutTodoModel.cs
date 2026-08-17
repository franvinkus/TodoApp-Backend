namespace TodoApp_Backend.DTOs
{
    public class PutTodoModel
    {   
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string startDate { get; set; } = string.Empty;
        public string endDate { get; set; } = string.Empty;

    }
}
