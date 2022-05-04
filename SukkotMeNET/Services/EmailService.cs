using SukkotMeNET.Configuration;
using System.Net;
using System.Net.Mail;

namespace SukkotMeNET.Services
{
    public class EmailService
    {
        readonly EmailConfig _EmailConfig;
        public EmailService(EmailConfig emailConfig)
        {
            _EmailConfig = emailConfig;
        }

        public bool Send(string to, string subject, string body)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(_EmailConfig.Address, _EmailConfig.Password)
                };

                var msg = new MailMessage
                {
                    From = new MailAddress(_EmailConfig.Address),
                    Sender = new MailAddress(_EmailConfig.Address),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body
                };
                msg.To.Add(new MailAddress(to));

                smtp.Send(msg);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
