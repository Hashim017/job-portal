using System.Net.Http.Json;
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
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

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
        var apiMode = !string.IsNullOrWhiteSpace(_settings.ApiKey);

        if (!_settings.Enabled ||
            string.IsNullOrWhiteSpace(_settings.FromAddress) ||
            (!apiMode && string.IsNullOrWhiteSpace(_settings.Host)))
        {
            _logger.LogInformation("Email is off. Skipped '{Subject}' to {To}.", subject, toAddress);
            return;
        }

        if (toAddress.EndsWith("@demo.com", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Skipped email to demo address {To}.", toAddress);
            return;
        }

        if (apiMode)
        {
            _ = Task.Run(() => SendApiAsync(toAddress, toName, subject, html, text));
        }
        else
        {
            _ = Task.Run(() => SendSmtpAsync(toAddress, toName, subject, html, text));
        }
    }

    private async Task SendApiAsync(string toAddress, string toName, string subject, string html, string text)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            request.Headers.Add("api-key", _settings.ApiKey);
            request.Content = JsonContent.Create(new
            {
                sender = new { name = _settings.FromName, email = _settings.FromAddress },
                to = new[] { new { email = toAddress, name = toName } },
                subject,
                htmlContent = html,
                textContent = text
            });

            using var response = await Http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent to {To}: {Subject}", toAddress, subject);
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Email API refused the message to {To}. Status {Status}. {Body}",
                    toAddress, (int)response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not send email to {To}.", toAddress);
        }
    }

    private async Task SendSmtpAsync(string toAddress, string toName, string subject, string html, string text)
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