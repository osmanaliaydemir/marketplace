namespace Application.Abstractions;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml);
    Task<bool> SendEmailAsync(string to, string cc, string subject, string body);
    Task<bool> SendEmailAsync(string to, string cc, string subject, string body, bool isHtml);
}
