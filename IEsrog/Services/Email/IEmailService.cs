namespace IEsrog.Services.Email;
public enum EmailType
{
    OrderConfirmation,
    ResetPassword
}

public record BroadcastEmail(string EmailAddress, string Name);

public interface IEmailService
{
    Task<bool> SendAsync(EmailType type, string body, string to, string? bcc = null);
    Task<bool> SendAsync(string[] to, string subject, string content);

}