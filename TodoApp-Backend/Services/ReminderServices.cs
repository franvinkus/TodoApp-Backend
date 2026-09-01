using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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
            var today = DateTime.UtcNow.Date;
            var maxLimit = today.AddDays(15);

            var pendingTodos = await _db.Todos
                .Include(x => x.User)
                .Where(x => !x.IsFinished && x.EndDate.Date >= today && x.EndDate.Date <= maxLimit)
                .ToListAsync();

            var groupedTodos = pendingTodos.GroupBy(t => t.User);

            foreach (var userGroup in groupedTodos)
            {
                var user = userGroup.Key;

                var toEmail = user.Email;
                var totalTasks = userGroup.Count();
                var subject = $"Ada {totalTasks} Tugas Todo untuk Hari Ini!";
                var body = $@"
                    <h3>Halo {user.Username ?? "!"}</h3>
                    <p>Jangan lupa, hari ini adalah tenggat waktu untuk menyelesaikan <strong>{totalTasks} tugas</strong> berikut:</p>
                    <ul>";
                
                foreach(var todos in userGroup)
                {
                    var remainingDays = (todos.EndDate.Date - DateTime.UtcNow.Date).Days;
                    body += $@"<li><strong>{todos.Title}</strong> dengan masing-masing sisa hari: <strong>{remainingDays} Hari</strong></li>";
                }

                body += $@"
                        </ul>
                        <p>Semangat mengerjakannya!</p>";


                Debug.WriteLine($"Mengirim email ke User ID {user.Id}: yang berisi: {totalTasks} hari ini!");
                await _em.SendEmail(toEmail, subject, body);
            }
            Debug.WriteLine("Selesai mengirim email harian!");
        }
    }
}
