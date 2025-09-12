using IEsrog.Configuration;
using System.Net;
using System.Net.Mail;
using IEsrog.Extensions;

namespace IEsrog.Services.Email;

internal class SmtpEmailService : IEmailService
{
    readonly ApplicationConfiguration _AppConfig;

    public SmtpEmailService(ApplicationConfiguration appConfig)
    {
        _AppConfig = appConfig;
    }

    const string from = "iesrog.yanky@gmail.com";
    
    public async Task<bool> SendAsync(EmailType type, string body, string to, string? bcc = null)
    {
        try
        {
            var apiKey = "hrhq hhua fvgw gmza";
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(from, apiKey),
                EnableSsl = true
            };

            var subjectStr = type switch
            {
                EmailType.OrderConfirmation => "Order Confirmation",
                EmailType.ResetPassword => "iEsrog Reset password",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var message = new MailMessage(from, to)
            {
                Subject = subjectStr,
                Body = body,
                IsBodyHtml = true
            };

            if(!string.IsNullOrWhiteSpace(bcc))
                message.Bcc.Add(bcc);
            
            if (type == EmailType.OrderConfirmation)
            {
                message.Bcc.Add("Iesrogonline@gmail.com");
            }

            await smtp.SendMailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public Task<bool> SendAsync(string[] to, string subject, string content)
    {
        throw new NotImplementedException();
    }
}