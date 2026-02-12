using IEsrog.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using System.Net;
using System.Net.Mail;

namespace IEsrog.Services.Email;

internal class SmtpEmailService : IEmailService
{
    readonly ApplicationConfiguration _AppConfig;
    readonly string _Host;
    readonly int _Port;
    readonly string _Key;
    readonly string _From;


    public SmtpEmailService(ApplicationConfiguration appConfig)
    {
        _AppConfig = appConfig;
        _Host = "smtp.gmail.com";
        _Port = 587;
        _Key = "hrhq hhua fvgw gmza";
        _From = "iesrog.yanky@gmail.com";
    }
    
    public async Task<bool> SendAsync(EmailType type, string body, string to, string? bcc = null)
    {
        try
        {
            var smtp = new SmtpClient(_Host, _Port)
            {
                Credentials = new NetworkCredential(_From, _Key),
                EnableSsl = true
            };

            var subjectStr = type switch
            {
                EmailType.OrderConfirmation => "Order Confirmation",
                EmailType.ResetPassword => "iEsrog Reset password",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var message = new MailMessage(_From, to)
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