using Microsoft.AspNetCore.Identity.UI.Services;

namespace Yash_Gems___Jewelleries.Services
{
    /// <summary>
    /// Email sender service for sending verification and password reset emails
    /// Uses SMTP configuration from appsettings.json
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (_configuration.GetValue<bool>("Email:UseMockSender"))
            {
                _logger.LogInformation(
                    "Mock Email Sender - To: {Email}, Subject: {Subject}, Message: {Message}",
                    email, subject, htmlMessage);
                return;
            }

            var smtpHost = _configuration["Email:Smtp:Host"];
            var smtpPort = _configuration.GetValue<int>("Email:Smtp:Port");
            var smtpUser = _configuration["Email:Smtp:Username"];
            var smtpPass = _configuration["Email:Smtp:Password"];
            var fromEmail = _configuration["Email:FromAddress"];
            var fromName = _configuration["Email:FromName"];

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(fromEmail))
            {
                _logger.LogWarning("Email configuration is incomplete. Email will not be sent.");
                return;
            }

            try
            {
                using var client = new System.Net.Mail.SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass)
                };

                var mailMessage = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", email);
                throw;
            }
        }
    }
}
