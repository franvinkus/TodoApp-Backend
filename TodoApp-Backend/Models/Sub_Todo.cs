namespace TodoApp_Backend.Models
{
    public class Sub_Todo
    {
        public Guid Id { get; set; } = new Guid();
        public int TodoId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
        public Todo? Todo { get; set; } = new Todo();
    }
}
