using FinalProject.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;


namespace FinalProject.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Send(string to, string subject, string body, string from = null)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _configuration.GetSection("Smtp:FromAddress").Value));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("Smtp:Server").Value, int.Parse(_configuration.GetSection("Smtp:Port").Value), SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("Smtp:FromAddress").Value, _configuration.GetSection("Smtp:Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
