namespace AspnetCoreMvcFull.Services
{
  public class StartupNotificationService : IHostedService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StartupNotificationService> _logger;

    public StartupNotificationService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<StartupNotificationService> logger)
    {
      _serviceProvider = serviceProvider;
      _configuration = configuration;
      _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Сайт запущено. Надсилання email адміністратору...");

      try
      {
        using (var scope = _serviceProvider.CreateScope())
        {
          var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
          var adminEmail = _configuration["AdminEmail"];

          if (string.IsNullOrEmpty(adminEmail))
          {
            _logger.LogWarning("Email адміністратора не налаштовано в appsettings.json");
            return;
          }

          var subject = " Сайт Materio успішно запущено";
          var body = GetEmailBody();

          await emailService.SendEmailAsync(adminEmail, subject, body);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Помилка при відправці email: {ex.Message}");
      }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Сайт зупинено");
      return Task.CompletedTask;
    }

    private string GetEmailBody()
    {
      var startupTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
      var serverName = Environment.MachineName;

      return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f4f4f4;
        }}
        .header {{
            background-color: #696cff;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: white;
            padding: 30px;
            border-radius: 0 0 5px 5px;
        }}
        .info-row {{
            padding: 10px 0;
            border-bottom: 1px solid #eee;
        }}
        .label {{
            font-weight: bold;
            color: #696cff;
        }}
        .footer {{
            text-align: center;
            padding: 20px;
            font-size: 12px;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🚀 Сайт успішно запущено</h1>
        </div>
        <div class='content'>
            <p>Шановний адміністраторе,</p>
            <p>Повідомляємо вас, що сайт <strong>Materio Admin Panel</strong> успішно запущено.</p>
            
            <div class='info-row'>
                <span class='label'> Час запуску:</span> {startupTime}
            </div>
            <div class='info-row'>
                <span class='label'> Сервер:</span> {serverName}
            </div>
            <div class='info-row'>
                <span class='label'> Статус:</span> Працює
            </div>
            
            <p style='margin-top: 20px;'>Всі системи функціонують нормально.</p>
            <p>Це автоматичне повідомлення, відповідати на нього не потрібно.</p>
        </div>
        <div class='footer'>
            &copy; 2026 Materio Admin System | Powered by ASP.NET Core MVC
        </div>
    </div>
</body>
</html>";
    }
  }
}
