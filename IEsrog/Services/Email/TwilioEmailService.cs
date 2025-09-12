using System.Net;
using IEsrog.Configuration;
using IEsrog.Extensions;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace IEsrog.Services.Email;



internal class TwilioEmailService : IEmailService
{
    readonly ApplicationConfiguration _AppConfig;
    readonly CyclicLoggerService _Logger;

    const string from = "Yanky@iesrog.com";
    
    public TwilioEmailService(
        ApplicationConfiguration emailConfig,
        CyclicLoggerService logger)
    {
        _AppConfig = emailConfig;
        _Logger = logger;
    }

    public async Task<bool> SendAsync(string[] to, string subject, string content)
    {
        try
        {
            var fromAddr = new EmailAddress(from);
            var toAddr = to
                .Select(x => new EmailAddress(x))
                .ToList();

            var subjects = to.Select(_ => subject).ToList();
            var substitutions = to
                .Select(_ => new Dictionary<string, string>())
                .ToList();

            var msg = MailHelper.CreateMultipleEmailsToMultipleRecipients(
                fromAddr, toAddr, subjects, content, null, substitutions);

            var apiKey = _AppConfig.EmailApiKey;
            var client = new SendGridClient(apiKey);
            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == HttpStatusCode.OK ||
                   response.StatusCode == HttpStatusCode.Accepted;
        }
        catch (Exception e)
        {
            _Logger.LogError($"Failed to send email to {to.ToCsv()}. Error: {e.ConcatMsg()}");
            return false;
        }
    }

    public async Task<bool> SendAsync(EmailType type, string body, string to, string? bcc = null)
    {
        try
        {
            var apiKey = _AppConfig.EmailApiKey;
            var client = new SendGridClient(apiKey);
            var fromAddr = new EmailAddress(from);
            var toAddr = new EmailAddress(to);

            var subjectStr = type switch
            {
                EmailType.OrderConfirmation => "Order Confirmation",
                EmailType.ResetPassword => "iEsrog Reset password",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var msg = MailHelper.CreateSingleEmail(fromAddr, toAddr, subjectStr, string.Empty, body);
            if (!string.IsNullOrEmpty(bcc) && bcc != to)
            {
                msg.AddBcc(new EmailAddress(bcc));
            }
            if (type == EmailType.OrderConfirmation)
            {
                //msg.AddBcc(new EmailAddress("chabad18@hotmail.com", "Yanky"));
                msg.AddBcc("Iesrogonline@gmail.com");
            }


            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == HttpStatusCode.OK ||
                   response.StatusCode == HttpStatusCode.Accepted;
        }
        catch (Exception ex)
        {
            _Logger.LogError($"Failed to send email to {to}. Error: {ex.ConcatMsg()}");
            return false;
        }
    }
}