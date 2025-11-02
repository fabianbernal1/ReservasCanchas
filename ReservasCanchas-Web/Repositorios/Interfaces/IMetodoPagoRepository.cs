using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReservasCanchas_Web.Repositorio
{
    public interface IMetodoPagoRepository
    {
        Task<IEnumerable<MetodoPago>> GetAllAsync();
        Task<MetodoPago?> GetByIdAsync(int id);
        Task AddAsync(MetodoPago metodo);
        Task UpdateAsync(MetodoPago metodo);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}