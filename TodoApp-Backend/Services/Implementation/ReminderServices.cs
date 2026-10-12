using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Diagnostics;
using TodoApp_Backend.Data;
using TodoApp_Backend.Repositories.Interface;
using TodoApp_Backend.Services.Interface;

namespace TodoApp_Backend.Services.Implementation
{
    public class ReminderServices: IReminderService
    {
        private readonly IEmailService _em;
        private readonly ITodoRepository _t;

        public ReminderServices(ITodoRepository t, IEmailService em)
        {
            _em = em;
            _t = t;
        }

        public async Task SendDailyReminder(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var maxLimit = today.AddDays(15);

            var pendingTodos = await _t.GetPendingTodos(today, maxLimit, cancellationToken);

            var groupedTodos = pendingTodos.GroupBy(t => t.User);

            var emailTasks = new List<Task>();

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
                emailTasks.Add(_em.SendEmail(toEmail, subject, body, cancellationToken));
            }

            await Task.WhenAll(emailTasks);

            Debug.WriteLine("Selesai mengirim email harian!");
        }
    }
}
