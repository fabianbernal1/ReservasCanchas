
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class MetodoPagoService : IMetodoPagoService
    {
        private readonly IMetodoPagoRepository _repo;
        public MetodoPagoService(IMetodoPagoRepository repo) => _repo = repo;

        public Task<IEnumerable<MetodoPago>> GetAllAsync() => _repo.GetAllAsync();

        public Task<MetodoPago?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task AddAsync(MetodoPago metodo) => _repo.AddAsync(metodo);

        public Task UpdateAsync(MetodoPago metodo) => _repo.UpdateAsync(metodo);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}