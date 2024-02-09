using System.Net;
using System.Net.Mail;
using static System.Net.WebRequestMethods;

namespace GirlsWantTheBestShop
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.mailersend.net", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("MS_i3UpCX@girlswantthebest.com", "Z5YLARytjkkFjW80")
            };
            return client.SendMailAsync(
                new MailMessage(from: "orders@girlswantthebest.com", to: email, subject, message)

                );


        } 


    }
}
