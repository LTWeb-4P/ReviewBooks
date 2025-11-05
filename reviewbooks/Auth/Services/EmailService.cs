using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace ReviewBooks.Auth.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromName { get; set; } = "Books Team";
    }

    public class EmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string html)
        {
            _logger.LogInformation($"[Email] Host: {_settings.Host}, Port: {_settings.Port}, User: {_settings.UserName?.Substring(0, Math.Min(5, _settings.UserName?.Length ?? 0))}..., HasPassword: {!string.IsNullOrEmpty(_settings.Password)}");

            if (string.IsNullOrEmpty(_settings.Host) || string.IsNullOrEmpty(_settings.UserName) || string.IsNullOrEmpty(_settings.Password))
            {
                throw new InvalidOperationException("Email settings are not configured properly. Check environment variables.");
            }

            try
            {
                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    EnableSsl = _settings.EnableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 10000 // 10 seconds timeout
                };

                _logger.LogInformation($"[Email] Connecting to SMTP server...");

                var mail = new MailMessage()
                {
                    From = new MailAddress(_settings.UserName, _settings.FromName),
                    Subject = subject,
                    Body = html,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                _logger.LogInformation($"[Email] Sending email to {to}...");
                await client.SendMailAsync(mail);
                _logger.LogInformation($"[Email] Successfully sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Email] Failed to send email: {ex.GetType().Name} - {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"[Email] Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}
