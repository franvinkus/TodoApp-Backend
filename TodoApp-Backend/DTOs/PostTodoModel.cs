namespace TodoApp_Backend.DTOs
{
    public class PostTodoModel
    {
        public string title { get; set; } = string.Empty;
        public string? description { get; set; } = string.Empty;
        public string startDate { get; set; } = string.Empty;
        public string endDate { get; set; } = string.Empty;
    }
}
