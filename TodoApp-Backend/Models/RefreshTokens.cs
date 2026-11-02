namespace TodoApp_Backend.Models
{
    public class RefreshTokens
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public Guid UserId { get; set; }
        public Users User { get; set; } = null!;
    }
}
