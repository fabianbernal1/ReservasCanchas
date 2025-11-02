
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;

namespace ReservasCanchas_Web.Servicios.Interfaces
{
    public interface IEstadoReservaService
    {
        Task<IEnumerable<EstadoReserva>> GetAllAsync();
        Task<EstadoReserva?> GetByIdAsync(int id);
        Task AddAsync(EstadoReserva estado);
        Task UpdateAsync(EstadoReserva estado);
        Task DeleteAsync(int id);
    }
}