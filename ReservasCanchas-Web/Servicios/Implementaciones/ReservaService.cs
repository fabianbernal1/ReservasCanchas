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

        // 1. NUEVO: Agregamos el servicio de Canchas
        private readonly ICanchaService _canchaService;

        private readonly IEmailService _emailService;
        private readonly ILogger<ReservaService> _logger;

        // 2. NUEVO: Lo inyectamos en el constructor
        public ReservaService(IReservaRepository repo,
                              IUsuarioService usuarioService,
                              ICanchaService canchaService, // <-- Aquí
                              IEmailService emailService,
                              ILogger<ReservaService> logger)
        {
            _repo = repo;
            _usuarioService = usuarioService;
            _canchaService = canchaService; // <-- Asignación
            _emailService = emailService;
            _logger = logger;
        }

        public Task<IEnumerable<Reserva>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Reserva?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public async Task AddAsync(Reserva reserva)
        {
            await _repo.AddAsync(reserva);

            try
            {
                var usuario = await _usuarioService.GetByIdAsync(reserva.UsuarioId);

                // 3. NUEVO: Buscamos la información de la cancha usando el ID
                var cancha = await _canchaService.GetByIdAsync(reserva.CanchaId);

                if (usuario != null && !string.IsNullOrEmpty(usuario.Correo))
                {
                    // Asignamos los objetos completos a la reserva para que el HTML pueda leer .Nombre
                    reserva.Usuario = usuario;
                    reserva.Cancha = cancha; // <-- ¡Esto hará que aparezca el nombre en el correo!

                    await _emailService.SendReservaConfirmationEmailAsync(reserva);

                    _logger.LogInformation("Correo enviado a {Correo}", usuario.Correo);
                }
                else
                {
                    _logger.LogWarning("Usuario no encontrado para reserva {ReservaId}", reserva.ReservaId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo al enviar correo para reserva {ReservaId}", reserva.ReservaId);
            }
        }

        public Task UpdateAsync(Reserva reserva) => _repo.UpdateAsync(reserva);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}