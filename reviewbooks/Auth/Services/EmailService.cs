using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

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
        public string SendGridApiKey { get; set; } = string.Empty;
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
            // Use SendGrid if API key is configured
            if (!string.IsNullOrEmpty(_settings.SendGridApiKey))
            {
                await SendEmailViaSendGridAsync(to, subject, html);
            }
            else
            {
                await SendEmailViaSmtpAsync(to, subject, html);
            }
        }

        private async Task SendEmailViaSendGridAsync(string to, string subject, string html)
        {
            _logger.LogInformation($"[Email SendGrid] Sending to {to}...");

            try
            {
                var client = new SendGridClient(_settings.SendGridApiKey);
                var from = new EmailAddress(_settings.UserName, _settings.FromName);
                var toAddress = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, null, html);

                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                    response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation($"[Email SendGrid] Successfully sent to {to}");
                }
                else
                {
                    var body = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"[Email SendGrid] Failed with status {response.StatusCode}: {body}");
                    throw new Exception($"SendGrid error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Email SendGrid] Exception: {ex.GetType().Name} - {ex.Message}");
                throw;
            }
        }

        private async Task SendEmailViaSmtpAsync(string to, string subject, string html)
        {
            _logger.LogInformation($"[Email SMTP] Host: {_settings.Host}, Port: {_settings.Port}, User: {_settings.UserName?.Substring(0, Math.Min(5, _settings.UserName?.Length ?? 0))}..., HasPassword: {!string.IsNullOrEmpty(_settings.Password)}");

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

                _logger.LogInformation($"[Email SMTP] Connecting to SMTP server...");

                var mail = new MailMessage()
                {
                    From = new MailAddress(_settings.UserName, _settings.FromName),
                    Subject = subject,
                    Body = html,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                _logger.LogInformation($"[Email SMTP] Sending email to {to}...");
                await client.SendMailAsync(mail);
                _logger.LogInformation($"[Email SMTP] Successfully sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Email SMTP] Failed: {ex.GetType().Name} - {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"[Email SMTP] Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}
