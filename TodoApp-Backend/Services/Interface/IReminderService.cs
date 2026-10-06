namespace TodoApp_Backend.Services.Interface
{
    public interface IReminderService
    {
        Task SendDailyReminder(CancellationToken cancellationToken);
    }
}
