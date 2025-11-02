
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservasCanchas_Web.Models;

namespace ReservasCanchas_Web.Servicios.Interfaces
{
    public interface IMetodoPagoService
    {
        Task<IEnumerable<MetodoPago>> GetAllAsync();
        Task<MetodoPago?> GetByIdAsync(int id);
        Task AddAsync(MetodoPago metodo);
        Task UpdateAsync(MetodoPago metodo);
        Task DeleteAsync(int id);
    }
}