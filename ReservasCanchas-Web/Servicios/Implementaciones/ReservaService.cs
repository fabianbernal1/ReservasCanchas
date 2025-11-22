
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repo;
        private readonly IEmailService _emailService;
       
        public ReservaService(IReservaRepository repo, IEmailService emailService) { 
            _repo = repo;
            _emailService = emailService;
        }

        public Task<IEnumerable<Reserva>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Reserva?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public async Task AddAsync(Reserva reserva)
        {
            await _repo.AddAsync(reserva);
            await _emailService.SendReservaConfirmationEmailAsync(reserva);
        }

        public Task UpdateAsync(Reserva reserva) => _repo.UpdateAsync(reserva);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}