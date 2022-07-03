using SukkotMeNET.Configuration;
using System.Net;
using System.Net.Mail;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class EmailService
    {
        readonly ApplicationConfiguration _AppConfig;
        public EmailService(ApplicationConfiguration emailConfig)
        {
            _AppConfig = emailConfig;
        }

        public async Task<bool> SendAsync(string subject, string body, params string[] to)
        {
            try
            {
                if (to.Length == 0)
                    throw new Exception();
                var smtp = new SmtpClient
                {
                    Host = _AppConfig.SmtpHost,
                    Port = _AppConfig.SmtpPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(_AppConfig.SmtpAddress, _AppConfig.SmtpPassword)
                };

                var msg = new MailMessage
                {
                    From = new MailAddress(_AppConfig.SmtpAddress),
                    Sender = new MailAddress(_AppConfig.SmtpAddress),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body
                };
                foreach (var t in to)
                {
                    msg.To.Add(new MailAddress(t));
                }

                await smtp.SendMailAsync(msg);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
