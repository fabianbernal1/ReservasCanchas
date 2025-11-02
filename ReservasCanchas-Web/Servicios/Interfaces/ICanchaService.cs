
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;

namespace ReservasCanchas_Web.Servicios.Interfaces
{
    public interface ICanchaService
    {
        Task<IEnumerable<Cancha>> GetAllAsync();
        Task<Cancha?> GetByIdAsync(int id);
        Task AddAsync(Cancha cancha);
        Task UpdateAsync(Cancha cancha);
        Task DeleteAsync(int id);
    }
}