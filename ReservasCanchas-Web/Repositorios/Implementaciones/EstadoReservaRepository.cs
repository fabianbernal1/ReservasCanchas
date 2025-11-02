using ReservasCanchas_Web.Data;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ReservasCanchas_Web.Repositorio
{
    public class EstadoReservaRepository : IEstadoReservaRepository
    {
        private readonly ApplicationDbContext _context;
        public EstadoReservaRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<EstadoReserva>> GetAllAsync() =>
            await _context.EstadoReserva.ToListAsync();

        public async Task<EstadoReserva?> GetByIdAsync(int id) =>
            await _context.EstadoReserva.FindAsync(id);

        public async Task AddAsync(EstadoReserva estado)
        {
            await _context.EstadoReserva.AddAsync(estado);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EstadoReserva estado)
        {
            _context.EstadoReserva.Update(estado);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.EstadoReserva.FindAsync(id);
            if (entity != null)
            {
                _context.EstadoReserva.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}