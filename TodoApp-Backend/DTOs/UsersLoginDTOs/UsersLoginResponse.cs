namespace TodoApp_Backend.DTOs
{
    public class UsersLoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
