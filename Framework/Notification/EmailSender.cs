using System.Net.Mail;
using System.Threading.Tasks;

namespace Framework.Notification
{
    public class EmailSender
    {
        private const string SERVER_ADDRESS = "relay.ceridian.com";
#pragma warning disable CS1998
        public static async Task SendEmailAsync(string subject, string body, string recipientEmail)
#pragma warning restore CS1998
        {
            string senderEmail = "TrainingAdmin@ceridian.com";
            var smtpClient = new SmtpClient(SERVER_ADDRESS)
            {
                Port = 25,
                EnableSsl = true,
                UseDefaultCredentials = true,
            };

            var mailMessage = new MailMessage(senderEmail, recipientEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
#pragma warning disable CS4014
            Task.Run(() => { smtpClient.SendMailAsync(mailMessage); }).ConfigureAwait(false);
#pragma warning restore CS4014
        }
    }
}
