using DnsClient.Internal;
using IEsrog.Extensions;
using SendGrid;
using SendGrid.Helpers.Mail;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using System.Net;
using IEsrog.Configuration;

namespace IEsrog.Services.Email;

public class BrevoEmailService : IEmailService
{
    readonly ILogger<BrevoEmailService> _Logger;
    readonly ApplicationConfiguration _Conf;

    readonly string _Host;
    readonly int _Port;
    readonly string _From;

    public BrevoEmailService(ILogger<BrevoEmailService> logger,
        ApplicationConfiguration conf)
    {
        _Logger = logger;
        _Conf = conf;
        _Host = "smtp-relay.brevo.com";
        _Port = 587;
        _From = "yanky@iesrog.com";
    }

    public async Task<bool> SendAsync(EmailType type, string body, string to, string? bcc = null)
    {
        try
        {
            var subjectStr = type switch
            {
                EmailType.OrderConfirmation => "Order Confirmation",
                EmailType.ResetPassword => "iEsrog Reset password",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            return await DoSendAsync(to, subjectStr, body);
        }
        catch (Exception e)
        {
            _Logger.LogError($"Failed to send email to {to.ToCsv()}. Error: {e.ConcatMsg()}");
            return false;
        }
    }

    async Task<bool> DoSendAsync(string to, string subject, string html)
    {
        try
        {
            sib_api_v3_sdk.Client.Configuration.Default.ApiKey["api-key"] = _Conf.EmailApiKey;

            var api = new TransactionalEmailsApi();
            var email = new SendSmtpEmail(
                to: new List<SendSmtpEmailTo>
                {
                    new(to, to)
                },
                subject: subject,
                htmlContent: html,
                sender: new SendSmtpEmailSender
                {
                    Email = _From,
                    Name = "iEsrog"
                }
            );

            await api.SendTransacEmailAsync(email);
            return true;
        }
        catch (Exception e)
        {
            _Logger.LogWarning($"Failed to send email. Error: {e}");
            return false;
        }
    }

    public async Task<bool> SendAsync(string[] to, string subject, string content)
    {
        try
        {
            foreach (var t in to)
            {
                await DoSendAsync(t, subject, content);
            }

            return true;
        }
        catch (Exception ex)
        {
            _Logger.LogError($"Failed to send email to {to}. Error: {ex.ConcatMsg()}");
            return false;
        }
    }
}