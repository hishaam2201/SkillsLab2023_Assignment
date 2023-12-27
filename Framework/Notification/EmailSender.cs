
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Framework.Notification
{
    public class EmailSender
    {
        private const string SERVER_ADDRESS = "relay.ceridian.com";
        public static async Task<bool> SendEmailAsync(string subject, string body, string recipientEmail)
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

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch(Exception) { throw; }
        }


    }
}
