using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Resend;
using System.Diagnostics;

namespace TodoApp_Backend.Services
{
    public class EmailServices
    {
        private readonly IConfiguration _config;

        public EmailServices(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var key = _config["EmailSettings:SmtpHost"];
            IResend resend = ResendClient.Create(key);

            var message = new EmailMessage
            {
                From = "noreply@todo.vincentkurnia.com",
                Subject = subject,
                HtmlBody = body
            };

            message.To.Add(toEmail);

            try
            {
                await resend.EmailSendAsync(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error pengiriman API: ", ex);
                throw; 
            }
        }

        //public async Task SendEmail(string toEmail,  string subject, string body)
        //{

        //    email.From.Add(new MailboxAddress("Vincent's Todo Tasks", _config["EmailSettings:SenderEmail"]));
        //    email.To.Add(MailboxAddress.Parse(toEmail));
        //    email.Subject = subject;

        //    var builder = new BodyBuilder { HtmlBody = body };
        //    email.Body = builder.ToMessageBody();

        //    using var smtp = new SmtpClient();
        //    try
        //    {
        //        smtp.Timeout = 120000;
        //        await smtp.ConnectAsync(_config["EmailSettings:SmtpHost"], int.Parse(_config["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTls);
        //        smtp.AuthenticationMechanisms.Remove("XOAUTH2");
        //        await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderPassword"]);


        //        await smtp.SendAsync(email);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error: ", ex);
        //        throw;
        //    }
        //    finally
        //    {
        //        await smtp.DisconnectAsync(true);
        //    }
        //}
    }
}
