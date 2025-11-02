using ReservasCanchas_Web.Models;


using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReservasCanchas_Web.Repositorio
{
    public interface IReservaRepository
    {
        Task<IEnumerable<Reserva>> GetAllAsync();
        Task<Reserva?> GetByIdAsync(int id);
        Task AddAsync(Reserva reserva);
        Task UpdateAsync(Reserva reserva);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}