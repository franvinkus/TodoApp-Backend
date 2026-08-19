using System.Diagnostics;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace TodoApp_Backend.Services
{
    public class EmailServices
    {
        private readonly IConfiguration _config;

        public EmailServices(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string toEmail,  string subject, string body)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Vincent's Todo Tasks", _config["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_config["EmailSettings:SmtpHost"], int.Parse(_config["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTls);
                smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderPassword"]);


                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: ", ex);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
