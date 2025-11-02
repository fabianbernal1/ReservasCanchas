
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class CanchaService : ICanchaService
    {
        private readonly ICanchaRepository _repo;
        public CanchaService(ICanchaRepository repo) => _repo = repo;

        public Task<IEnumerable<Cancha>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Cancha?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task AddAsync(Cancha cancha) => _repo.AddAsync(cancha);

        public Task UpdateAsync(Cancha cancha) => _repo.UpdateAsync(cancha);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}