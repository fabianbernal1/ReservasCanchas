using ReservasCanchas_Web.Data;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReservasCanchas_Web.Repositorio
{
    public class PagoRepository : IPagoRepository
    {
        private readonly ApplicationDbContext _context;
        public PagoRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Pago>> GetAllAsync() =>
            await _context.Pago
                .Include(p => p.Reserva)
                .Include(p => p.MetodoPago)
                .ToListAsync();

        public async Task<Pago?> GetByIdAsync(int id) =>
            await _context.Pago
                .Include(p => p.Reserva)
                .Include(p => p.MetodoPago)
                .FirstOrDefaultAsync(p => p.PagoId == id);

        public async Task AddAsync(Pago pago)
        {
            await _context.Pago.AddAsync(pago);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pago pago)
        {
            _context.Pago.Update(pago);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Pago.FindAsync(id);
            if (entity != null)
            {
                _context.Pago.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}