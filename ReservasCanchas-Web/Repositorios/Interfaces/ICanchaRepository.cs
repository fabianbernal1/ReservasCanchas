using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReservasCanchas_Web.Repositorio
{
    public interface ICanchaRepository
    {
        Task<IEnumerable<Cancha>> GetAllAsync();
        Task<Cancha?> GetByIdAsync(int id);
        Task AddAsync(Cancha cancha);
        Task UpdateAsync(Cancha cancha);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}