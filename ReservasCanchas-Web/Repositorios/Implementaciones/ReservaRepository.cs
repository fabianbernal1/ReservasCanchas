using ReservasCanchas_Web.Data;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ReservasCanchas_Web.Repositorio
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly ApplicationDbContext _context;
        public ReservaRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Reserva>> GetAllAsync() =>
            await _context.Reserva
                .Include(r => r.Usuario)
                .Include(r => r.Cancha)
                .Include(r => r.Estado)
                .ToListAsync();

        public async Task<Reserva?> GetByIdAsync(int id) =>
            await _context.Reserva
                .Include(r => r.Usuario)
                .Include(r => r.Cancha)
                .Include(r => r.Estado)
                .FirstOrDefaultAsync(r => r.ReservaId == id);

        public async Task AddAsync(Reserva reserva)
        {
            await _context.Reserva.AddAsync(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reserva reserva)
        {
            _context.Reserva.Update(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Reserva.FindAsync(id);
            if (entity != null)
            {
                _context.Reserva.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}