using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration; // Necesario para IConfiguration
using MimeKit;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Servicios.Interfaces;
using System.Net.Mail;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        // El constructor ahora recibe la configuración
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendReservaConfirmationEmailAsync(Reserva reserva)
        {
            // 1. Obtener datos de la reserva y configuración
            string host = _config["EmailSettings:Host"] ?? throw new InvalidOperationException("Host no configurado.");
            int port = int.Parse(_config["EmailSettings:Port"] ?? "587");
            string username = _config["EmailSettings:Username"] ?? throw new InvalidOperationException("Usuario de correo no configurado.");
            string password = _config["EmailSettings:Password"] ?? throw new InvalidOperationException("Contraseña de correo no configurada.");
            string fromEmail = _config["EmailSettings:FromEmail"] ?? "no-reply@reservascanchas.com";

            // Suponiendo que el modelo Usuario tiene la propiedad Email
            string toEmail = reserva.Usuario?.Correo ?? throw new InvalidOperationException("Email de usuario no disponible.");
            string subject = $"Confirmación de Reserva de Cancha #{reserva.ReservaId}";

            // 2. Construir el cuerpo del correo
            string body = $"Hola {reserva.Usuario?.Nombre}," +
                          $"<p>Tu reserva de la cancha <b>{reserva.Cancha?.Nombre}</b> ha sido confirmada.</p>" +
                          $"Detalles: Fecha {reserva.FechaReserva.ToShortDateString()}, " +
                          $"Horario de {reserva.HoraInicio} a {reserva.HoraFin}." +
                          $"<p>¡Gracias por tu reserva!</p>";

            // 3. Lógica real de envío (Usando MailKit)
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Reservas Canchas", fromEmail));
            email.To.Add(new MailboxAddress(reserva.Usuario?.Nombre, toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                // La autenticación es crucial y debe coincidir con el servidor
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
        }
    }
}