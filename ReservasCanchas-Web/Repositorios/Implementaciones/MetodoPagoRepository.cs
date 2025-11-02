using ReservasCanchas_Web.Data;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReservasCanchas_Web.Repositorio
{
    public class MetodoPagoRepository : IMetodoPagoRepository
    {
        private readonly ApplicationDbContext _context;
        public MetodoPagoRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<MetodoPago>> GetAllAsync() =>
            await _context.MetodoPago.ToListAsync();

        public async Task<MetodoPago?> GetByIdAsync(int id) =>
            await _context.MetodoPago.FindAsync(id);

        public async Task AddAsync(MetodoPago metodo)
        {
            await _context.MetodoPago.AddAsync(metodo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MetodoPago metodo)
        {
            _context.MetodoPago.Update(metodo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.MetodoPago.FindAsync(id);
            if (entity != null)
            {
                _context.MetodoPago.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}