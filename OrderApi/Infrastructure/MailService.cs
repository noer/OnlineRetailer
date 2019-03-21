using System.Net;
using System.Net.Mail;

namespace OrderApi.Infrastructure
{
    public class MailService : IMailService
    {
        public void sendMessage(string to, string subject, string message)
        {
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("8290a89e1403cc", "ac9cbda0bbae65"),
                EnableSsl = true
            };
            client.Send("OrderAPI", to, subject, message);
        }
    }
}