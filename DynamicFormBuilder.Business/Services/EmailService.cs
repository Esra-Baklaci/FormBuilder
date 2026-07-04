using System.Net;
using System.Net.Mail;
using System.Text;
using DynamicFormBuilder.Business.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DynamicFormBuilder.Business.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendFormSubmissionNotificationAsync(
        string toEmail,
        string formTitle,
        DateTime submittedAt,
        IReadOnlyList<(string Label, string Value)> values)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("E-posta bildirimi devre dışı. Form: {FormTitle}", formTitle);
            return;
        }

        try
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            var body = new StringBuilder();
            body.AppendLine($"<h3>{formTitle} - Yeni Yanıt</h3>");
            body.AppendLine($"<p><strong>Gönderim:</strong> {submittedAt.ToLocalTime():g}</p>");
            body.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
            body.AppendLine("<tr><th>Alan</th><th>Değer</th></tr>");

            foreach (var (label, value) in values)
            {
                body.AppendLine($"<tr><td>{WebUtility.HtmlEncode(label)}</td><td>{WebUtility.HtmlEncode(value)}</td></tr>");
            }

            body.AppendLine("</table>");

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = $"[{formTitle}] Yeni Form Yanıtı",
                Body = body.ToString(),
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            _logger.LogInformation("E-posta bildirimi gönderildi: {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "E-posta gönderimi başarısız: {ToEmail}", toEmail);
        }
    }
}
