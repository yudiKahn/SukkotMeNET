using System.Net;
using System.Net.Mail;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using IEsrog.Configuration;
using IEsrog.Models;

namespace IEsrog.Services
{
    public class EmailService
    {
        readonly ApplicationConfiguration _AppConfig;
        readonly IAmazonSimpleEmailService _SesService;

        public EmailService(
            ApplicationConfiguration emailConfig,
            IAmazonSimpleEmailService sesService)
        {
            _AppConfig = emailConfig;
            _SesService = sesService;
        }

        public async Task<bool> SendAsync(string subject, string body, params string[] to)
        {
            return await SendAsync(subject, body, null, to);
        }

        public async Task<bool> SendAsync(string subject, string body, string? bcc, string[] to)
        {

            try
            {
                if (to.Length == 0)
                    throw new Exception();

                var req = new SendEmailRequest(
                    "www.iesrog.com",
                    new Destination()
                    {
                        ToAddresses = to.ToList(),
                        BccAddresses = bcc is null ? [] : [bcc]
                    },
                    new Message(new Content(subject), new Body()
                    {
                        Html = new Content(body)
                    }));

                var resp = await _SesService.SendEmailAsync(req);
                return resp.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
