using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace ReservasCanchas_Web.Repositorio
{
    public interface IEstadoReservaRepository
    {
        Task<IEnumerable<EstadoReserva>> GetAllAsync();
        Task<EstadoReserva?> GetByIdAsync(int id);
        Task AddAsync(EstadoReserva estado);
        Task UpdateAsync(EstadoReserva estado);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}