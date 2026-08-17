using System.Text.Json.Serialization;

namespace TodoApp_Backend.DTOs
{
    public class GetTodoModel
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string createdAt { get; set; } = string.Empty;
        public string finishedAt { get; set; } = string.Empty;
        public string startDate {  get; set; } = string.Empty;
        public string endDate { get; set; } = string.Empty;
        public bool isCompleted { get; set; }

    }
}
