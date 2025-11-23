using System.Net;
using System.Net.Mail;

namespace WebApplication3.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"])))
            {
                client.Credentials = new NetworkCredential(_config["Smtp:User"], _config["Smtp:Pass"]);
                client.EnableSsl = true;
                await client.SendMailAsync(new MailMessage(_config["Smtp:User"], email, subject, message) { IsBodyHtml = true });
            }
        }
    }
}
