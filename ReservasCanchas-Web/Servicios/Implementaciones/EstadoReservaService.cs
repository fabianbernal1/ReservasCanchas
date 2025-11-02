
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class EstadoReservaService : IEstadoReservaService
    {
        private readonly IEstadoReservaRepository _repo;
        public EstadoReservaService(IEstadoReservaRepository repo) => _repo = repo;

        public Task<IEnumerable<EstadoReserva>> GetAllAsync() => _repo.GetAllAsync();

        public Task<EstadoReserva?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task AddAsync(EstadoReserva estado) => _repo.AddAsync(estado);

        public Task UpdateAsync(EstadoReserva estado) => _repo.UpdateAsync(estado);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}