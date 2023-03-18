using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class EmailService:IEmail
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(request.From));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Text) { Text = request.Body };

            using var smtp = new SmtpClient();
            
            smtp.Connect(_config.GetSection("SmtpHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("SmtpAccountUserName").Value, _config.GetSection("SmtpAccountPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
