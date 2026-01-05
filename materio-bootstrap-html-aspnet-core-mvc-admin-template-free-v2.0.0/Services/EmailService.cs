using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using AspnetCoreMvcFull.Models.Settings;

namespace AspnetCoreMvcFull.Services
{
  public class EmailService : IEmailService
  {
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
      _emailSettings = emailSettings.Value;
      _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
      try
      {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
          HtmlBody = body
        };

        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
          await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort,
              _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

          await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
          await client.SendAsync(message);
          await client.DisconnectAsync(true);
        }

        _logger.LogInformation($"Email успішно відправлено на адресу: {toEmail}");
      }
      catch (Exception ex)
      {
        _logger.LogError($"Помилка при відправці email: {ex.Message}");
        throw;
      }
    }
  }
}
