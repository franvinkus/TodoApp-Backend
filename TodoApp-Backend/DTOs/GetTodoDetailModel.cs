namespace TodoApp_Backend.DTOs
{
    public class GetTodoDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string FinishedAt { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string TodoPriority { get; set; } = string.Empty;
        public List<GetSubTodoModel> SubTodos { get; set; } = new List<GetSubTodoModel>();
    }

    public class GetSubTodoModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; }
    }
}
