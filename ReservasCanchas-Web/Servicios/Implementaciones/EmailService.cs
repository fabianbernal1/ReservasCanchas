using Microsoft.Extensions.Configuration;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Servicios.Interfaces;
using System.Threading.Tasks;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender; // 1. Declaramos la dependencia del servicio de envío real

        // 2. Inyectamos IEmailSender en el constructor
        public EmailService(IConfiguration config, IEmailSender emailSender)
        {
            _config = config;
            _emailSender = emailSender;
        }

        public async Task SendReservaConfirmationEmailAsync(Reserva reserva)
        {
            // Validar correo
            string toEmail = reserva.Usuario?.Correo ?? throw new InvalidOperationException("Email de usuario no disponible.");
            string subject = $"Confirmación de Reserva de Cancha #{reserva.ReservaId}";

            // 3. Construir el cuerpo del correo (Tu HTML estilizado)
            // Nota: Aquí ya no necesitamos credenciales ni configuración de host, eso lo maneja IEmailSender
            string body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8' />
                    <title>Confirmación de Reserva</title>
                </head>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width: 600px; margin: auto; background-color: #ffffff; border: 1px solid #dddddd; border-radius: 8px;'>
                        <tr>
                            <td style='background-color: #007bff; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                                <h1 style='color: white; margin: 0; font-size: 24px;'>Confirmación de Reserva de Cancha</h1>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding: 30px;'>
                                <h2 style='color: #333333; margin-top: 0;'>¡Hola, {reserva.Usuario?.Nombre}!</h2>
                                
                                <p style='font-size: 16px; color: #555555;'>Tu reserva ha sido <strong style='color: #007bff;'>confirmada exitosamente</strong>. Guarda este detalle:</p>

                                <table width='100%' cellpadding='10' cellspacing='0' border='0' style='border: 1px solid #eeeeee; margin-top: 20px;'>
                                    <tr>
                                        <td style='background-color: #f9f9f9; width: 40%; font-weight: bold;'>ID de Reserva:</td>
                                        <td style='background-color: #f9f9f9;'>{reserva.ReservaId}</td>
                                    </tr>
                                    <tr>
                                        <td style='width: 40%; font-weight: bold;'>Cancha:</td>
                                        <td>{reserva.Cancha?.Nombre}</td>
                                    </tr>
                                    <tr>
                                        <td style='background-color: #f9f9f9; width: 40%; font-weight: bold;'>Fecha y Hora:</td>
                                        <td style='background-color: #f9f9f9;'><strong style='color: #28a745;'>{reserva.FechaReserva.ToShortDateString()}</strong> de {reserva.HoraInicio} a {reserva.HoraFin}</td>
                                    </tr>
                                </table>
                                
                                <p style='margin-top: 25px; text-align: center;'>
                                    <a href='#' style='display: inline-block; padding: 10px 20px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>Ver Detalles Completos</a>
                                </p>

                                <p style='margin-top: 30px; font-size: 16px; color: #555555;'>¡Te esperamos en la cancha!</p>
                            </td>
                        </tr>
                        <tr>
                            <td style='background-color: #eeeeee; padding: 15px; text-align: center; font-size: 12px; color: #777777; border-radius: 0 0 8px 8px;'>
                                Enviado desde no-reply@reservascanchas.com
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

            // 4. DELEGAMOS el envío al servicio corregido SmtpEmailSender
            await _emailSender.SendEmailAsync(toEmail, subject, body);
        }
    }
}