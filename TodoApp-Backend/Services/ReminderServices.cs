using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TodoApp_Backend.Data;

namespace TodoApp_Backend.Services
{
    public class ReminderServices
    {
        private readonly TodoAppDbContext _db;
        private readonly EmailServices _em;

        public ReminderServices(TodoAppDbContext db, EmailServices em)
        {
            _db = db;
            _em = em;
        }

        public async Task SendDailyReminder()
        {
            var pendingTodos = await _db.Todos
                .Include(x => x.User)
                .Where(x => !x.IsFinished && x.EndDate.Date != DateTime.UtcNow.Date)
                .ToListAsync();

            foreach (var todo in pendingTodos)
            {
                var remainingDays = (todo.EndDate.Date - DateTime.UtcNow.Date).Days;
                var taskTitle = todo.Title;

                var toEmail = todo.User.Email;
                var subject = $"Task Todo: {taskTitle}";
                var body = $@"
                    <h3>Halo!</h3>
                    <p>Jangan lupa, hari ini adalah tenggat waktu untuk menyelesaikan tugas: <strong>{todo.Title}</strong>.</p>
                    <p>Sisa waktu pengerjaan: <strong>{remainingDays}</strong><p>
                    <p>Semangat mengerjakannya!</p>";


                Debug.WriteLine($"Mengirim email ke User ID {todo.UserId}: Jangan lupa kerjakan '{todo.Title}' hari ini!");
                await _em.SendEmail(toEmail, subject, body);
            }
            Debug.WriteLine("Selesai mengirim email harian!");
        }
    }
}
