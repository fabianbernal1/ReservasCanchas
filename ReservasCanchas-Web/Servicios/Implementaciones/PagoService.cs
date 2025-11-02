
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _repo;
        public PagoService(IPagoRepository repo) => _repo = repo;

        public Task<IEnumerable<Pago>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Pago?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task AddAsync(Pago pago) => _repo.AddAsync(pago);

        public Task UpdateAsync(Pago pago) => _repo.UpdateAsync(pago);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}