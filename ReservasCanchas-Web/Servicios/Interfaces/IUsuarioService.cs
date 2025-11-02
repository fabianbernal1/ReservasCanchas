
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;

namespace ReservasCanchas_Web.Servicios.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
    }
}