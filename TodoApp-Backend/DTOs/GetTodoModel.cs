namespace TodoApp_Backend.DTOs
{
    public class GetTodoModel
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string createdAt { get; set; } = string.Empty;
        public string finishedAt { get; set; } = string.Empty;
        public bool isCompleted { get; set; }

    }
}
