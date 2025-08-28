using Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;

namespace Infrastructure.Services;

public sealed class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        return await SendEmailAsync(to, subject, body, false);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml)
    {
        try
        {
            _logger.LogInformation("Sending email to: {To}, Subject: {Subject}", to, subject);

            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var smtpServer = smtpSettings["Server"] ?? "localhost";
            var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
            var smtpUsername = smtpSettings["Username"] ?? "";
            var smtpPassword = smtpSettings["Password"] ?? "";
            var fromEmail = smtpSettings["FromEmail"] ?? "noreply@marketplace.com";
            var fromName = smtpSettings["FromName"] ?? "Marketplace";

            using var client = new SmtpClient(smtpServer, smtpPort);
            
            if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;
            }

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(to);

            await client.SendMailAsync(message);

            _logger.LogInformation("Email sent successfully to: {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to: {To}", to);
            return false;
        }
    }

    public async Task<bool> SendEmailAsync(string to, string cc, string subject, string body)
    {
        return await SendEmailAsync(to, cc, subject, body, false);
    }

    public async Task<bool> SendEmailAsync(string to, string cc, string subject, string body, bool isHtml)
    {
        try
        {
            _logger.LogInformation("Sending email to: {To}, CC: {CC}, Subject: {Subject}", to, cc, subject);

            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var smtpServer = smtpSettings["Server"] ?? "localhost";
            var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
            var smtpUsername = smtpSettings["Username"] ?? "";
            var smtpPassword = smtpSettings["Password"] ?? "";
            var fromEmail = smtpSettings["FromEmail"] ?? "noreply@marketplace.com";
            var fromName = smtpSettings["FromName"] ?? "Marketplace";

            using var client = new SmtpClient(smtpServer, smtpPort);
            
            if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;
            }

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(to);
            
            if (!string.IsNullOrEmpty(cc))
                message.CC.Add(cc);

            await client.SendMailAsync(message);

            _logger.LogInformation("Email sent successfully to: {To}, CC: {CC}", to, cc);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to: {To}, CC: {CC}", to, cc);
            return false;
        }
    }
}
