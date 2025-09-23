using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using ProjetoMongoDB.Settings;


namespace ProjetoMongoDB.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html);

            // Conexão
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            // Autenticação
            await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

            // Envio de e-mail
            await smtp.SendAsync(email);

            // Desconexão
            await smtp.DisconnectAsync(true);
        }
    }
}
