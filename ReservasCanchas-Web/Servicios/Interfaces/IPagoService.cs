
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;

namespace ReservasCanchas_Web.Servicios.Interfaces
{
    public interface IPagoService
    {
        Task<IEnumerable<Pago>> GetAllAsync();
        Task<Pago?> GetByIdAsync(int id);
        Task AddAsync(Pago pago);
        Task UpdateAsync(Pago pago);
        Task DeleteAsync(int id);
    }
}