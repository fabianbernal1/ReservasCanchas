
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
        public ReservaService(IReservaRepository repo) => _repo = repo;

        public Task<IEnumerable<Reserva>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Reserva?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task AddAsync(Reserva reserva) => _repo.AddAsync(reserva);

        public Task UpdateAsync(Reserva reserva) => _repo.UpdateAsync(reserva);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}