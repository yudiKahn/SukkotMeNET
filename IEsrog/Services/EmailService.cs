using System.Net;
using IEsrog.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace IEsrog.Services;

public class EmailService
{
    readonly ApplicationConfiguration _AppConfig;

    public EmailService(
        ApplicationConfiguration emailConfig)
    {
        _AppConfig = emailConfig;
    }

    public async Task<bool> SendAsync(string subject, string body, string to)
    {
        return await SendAsync(subject, body, null, to);
    }

    public async Task<bool> SendAsync(string subject, string body, string? bcc, string to)
    {

        try
        {
            const string from = "Yanky@iesrog.com";

            var apiKey = _AppConfig.EmailApiKey;
            var client = new SendGridClient(apiKey);
            var fromAddr = new EmailAddress(from);
            var toAddr = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(fromAddr, toAddr, subject, string.Empty, body);
            msg.AddBcc(new EmailAddress(bcc));
            
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