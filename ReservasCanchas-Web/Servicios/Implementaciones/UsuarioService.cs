
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Servicios.Implementaciones
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        public UsuarioService(IUsuarioRepository repo) => _repo = repo;

        public Task<IEnumerable<Usuario>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Usuario?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task AddAsync(Usuario usuario) => _repo.AddAsync(usuario);

        public Task UpdateAsync(Usuario usuario) => _repo.UpdateAsync(usuario);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}