using SukkotMeNET.Configuration;
using System.Net;
using System.Net.Mail;

namespace SukkotMeNET.Services
{
    public class EmailService
    {
        readonly ApplicationConfiguration _AppConfig;
        public EmailService(ApplicationConfiguration emailConfig)
        {
            _AppConfig = emailConfig;
        }

        public async Task<bool> SendAsync(string to, string subject, string body)
        {
            try
            {
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
                msg.To.Add(new MailAddress(to));

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
