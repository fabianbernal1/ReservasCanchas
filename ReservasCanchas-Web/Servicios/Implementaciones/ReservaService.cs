using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;
using ReservasCanchas_Web.Servicios.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repo;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ReservaService> _logger;

        public ReservaService(IReservaRepository repo, IUsuarioService usuarioService, IEmailSender emailSender, ILogger<ReservaService> logger)
        {
            _repo = repo;
            _usuarioService = usuarioService;
            _emailSender = emailSender;
            _logger = logger;
        }

        public Task<IEnumerable<Reserva>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Reserva?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public async Task AddAsync(Reserva reserva)
        {
            // El repositorio ya guarda los cambios internamente en AddAsync,
            // por eso NO debemos llamar SaveChangesAsync aquí (evitamos doble guardado).
            await _repo.AddAsync(reserva);

            // Intentar enviar correo al usuario asociado (si existe). Loguear resultados.
            try
            {
                var usuario = await _usuarioService.GetByIdAsync(reserva.UsuarioId);
                if (usuario != null && !string.IsNullOrEmpty(usuario.Correo))
                {
                    var subject = "Confirmación de reserva";
                    // Usar ToString para formatear TimeSpan y evitar errores de escape en interpolación
                    var horaInicio = reserva.HoraInicio.ToString(@"hh\:mm");
                    var horaFin = reserva.HoraFin.ToString(@"hh\:mm");

                    var body = $@"<p>Hola {usuario.Nombre},</p>
                                  <p>Tu reserva (ID: {reserva.ReservaId}) para la fecha {reserva.FechaReserva:dd/MM/yyyy} de {horaInicio} a {horaFin} fue registrada correctamente.</p>
                                  <p>Gracias.</p>";

                    await _emailSender.SendEmailAsync(usuario.Correo, subject, body);
                    _logger.LogInformation("Correo de confirmación enviado a {Correo} para reserva {ReservaId}", usuario.Correo, reserva.ReservaId);
                }
                else
                {
                    _logger.LogWarning("No se encontró usuario o correo vacío para la reserva {ReservaId}", reserva.ReservaId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo al enviar correo de confirmación para la reserva {ReservaId}", reserva.ReservaId);
                // No rethrow para no romper el flujo de guardado.
            }
        }

        public Task UpdateAsync(Reserva reserva) => _repo.UpdateAsync(reserva);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}