using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ReservasCanchas_Web.Servicios.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class SmtpOptions
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string? Username { get; set; } = "ewcshnhm461@gmail.com";
        public string? Password { get; set; } = "";
        public string From { get; set; } = "ewcshnhm461@gmail.com";
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation("Preparando envío de correo a {To} (asunto: {Subject})", to, subject);

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_options.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                var secure = _options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                await client.ConnectAsync(_options.Host, _options.Port, secure);
                if (!string.IsNullOrEmpty(_options.Username) && !string.IsNullOrEmpty(_options.Password))
                {
                    await client.AuthenticateAsync(_options.Username, _options.Password);
                }
                else
                {
                    _logger.LogWarning("Credenciales SMTP no configuradas. Host={Host}, Port={Port}", _options.Host, _options.Port);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                _logger.LogInformation("Correo enviado correctamente a {To}", to);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo a {To}", to);
                throw;
            }
        }
    }
}