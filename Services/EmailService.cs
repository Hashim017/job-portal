using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace JobPortal.Services;

public interface IEmailService
{
    void Queue(string toAddress, string toName, string subject, string html, string text);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    // Sends in the background so the page does not wait for the mail server
    public void Queue(string toAddress, string toName, string subject, string html, string text)
    {
        if (!_settings.Enabled ||
            string.IsNullOrWhiteSpace(_settings.Host) ||
            string.IsNullOrWhiteSpace(_settings.FromAddress))
        {
            _logger.LogInformation("Email is off. Skipped '{Subject}' to {To}.", subject, toAddress);
            return;
        }

        if (toAddress.EndsWith("@demo.com", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Skipped email to demo address {To}.", toAddress);
            return;
        }

        _ = Task.Run(() => SendAsync(toAddress, toName, subject, html, text));
    }

    private async Task SendAsync(string toAddress, string toName, string subject, string html, string text)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            message.To.Add(new MailboxAddress(toName, toAddress));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = html, TextBody = text }.ToMessageBody();

            using var client = new SmtpClient();
            client.Timeout = 15000;

            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            if (!string.IsNullOrWhiteSpace(_settings.User))
            {
                await client.AuthenticateAsync(_settings.User, _settings.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {To}: {Subject}", toAddress, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not send email to {To}.", toAddress);
        }
    }
}