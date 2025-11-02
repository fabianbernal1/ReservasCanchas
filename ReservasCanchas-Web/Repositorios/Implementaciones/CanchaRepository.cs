using ReservasCanchas_Web.Data;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ReservasCanchas_Web.Repositorio
{
    public class CanchaRepository : ICanchaRepository
    {
        private readonly ApplicationDbContext _context;
        public CanchaRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Cancha>> GetAllAsync() =>
            await _context.Cancha.ToListAsync();

        public async Task<Cancha?> GetByIdAsync(int id) =>
            await _context.Cancha.FindAsync(id);

        public async Task AddAsync(Cancha cancha)
        {
            await _context.Cancha.AddAsync(cancha);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cancha cancha)
        {
            _context.Cancha.Update(cancha);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Cancha.FindAsync(id);
            if (entity != null)
            {
                _context.Cancha.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}