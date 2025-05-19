using System.Net;
using IEsrog.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace IEsrog.Services;

public enum EmailType
{
    OrderConfirmation,
    ResetPassword
}

public class EmailService
{
    readonly ApplicationConfiguration _AppConfig;

    public EmailService(
        ApplicationConfiguration emailConfig)
    {
        _AppConfig = emailConfig;
    }

    public async Task<bool> SendAsync(EmailType subject, string body, string to)
    {
        return await SendAsync(subject, body, null, to);
    }

    public async Task<bool> SendAsync(EmailType subject, string body, string? bcc, string to)
    {

        try
        {
            const string from = "Yanky@iesrog.com";

            var apiKey = _AppConfig.EmailApiKey;
            var client = new SendGridClient(apiKey);
            var fromAddr = new EmailAddress(from);
            var toAddr = new EmailAddress(to);
            
            var subjectStr = subject switch
            {
                EmailType.OrderConfirmation => "Order Confirmation",
                EmailType.ResetPassword => "iEsrog Reset password",
                _ => throw new ArgumentOutOfRangeException(nameof(subject), subject, null)
            };

            var msg = MailHelper.CreateSingleEmail(fromAddr, toAddr, subjectStr, string.Empty, body);
            if(!string.IsNullOrEmpty(bcc))
            {
                msg.AddBcc(new EmailAddress(bcc));
            }
            if (subject == EmailType.OrderConfirmation)
            {
                msg.AddBcc("chabad18@hotmail.com");
                msg.AddBcc("Iesrogonline@gmail.com");
            }


            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == HttpStatusCode.OK || 
                   response.StatusCode == HttpStatusCode.Accepted;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}