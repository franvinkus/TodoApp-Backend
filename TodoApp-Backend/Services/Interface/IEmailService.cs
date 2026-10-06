namespace TodoApp_Backend.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmail(string toEmail, string subject, string body, CancellationToken cancellationToken);
    }
}
