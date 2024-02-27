using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace Class_system_Backstage_pj.Areas.ordering_system.Data
{
    public class Emailsender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("a840123b@gmail.com");
            mail.Subject = subject;
            mail.Body = htmlMessage;
            mail.To.Add(email);
            mail.IsBodyHtml = true;
            //--------獲得smtp認證
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false; //是用來指示是否使用預設的網路憑證
            smtpClient.Credentials = new NetworkCredential("a840123b@gmail.com", "skbp irxt hyds fnlp");
            smtpClient.EnableSsl = true; /// 如果你的 SMTP 伺服器支援 SSL，可以啟用它
            smtpClient.Send(mail);
        }
    }
}
