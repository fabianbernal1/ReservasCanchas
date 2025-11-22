using ReservasCanchas_Web.Data;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Repositorio;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using ReservasCanchas_Web.Servicios.Exceptions; // <-- para ReservaSolapadaException

namespace ReservasCanchas_Web.Repositorio
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public ReservaRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = _context.Database.GetDbConnection()?.ConnectionString
                                ?? configuration.GetConnectionString("DefaultConnection")
                                ?? string.Empty;
        }

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

        // Inserta usando el SP; captura SqlException para convertir a excepción de dominio
        public async Task AddAsync(Reserva reserva)
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Cadena de conexión no configurada. Configure 'ConnectionStrings:DefaultConnection'.");

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sp_GuardarReserva", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@ReservaId", SqlDbType.Int) { Value = 0 });
            cmd.Parameters.Add(new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = reserva.UsuarioId });
            cmd.Parameters.Add(new SqlParameter("@CanchaId", SqlDbType.Int) { Value = reserva.CanchaId });
            cmd.Parameters.Add(new SqlParameter("@FechaReserva", SqlDbType.DateTime2) { Value = reserva.FechaReserva });
            cmd.Parameters.Add(new SqlParameter("@HoraInicio", SqlDbType.Time) { Value = reserva.HoraInicio });
            cmd.Parameters.Add(new SqlParameter("@HoraFin", SqlDbType.Time) { Value = reserva.HoraFin });
            cmd.Parameters.Add(new SqlParameter("@EstadoId", SqlDbType.Int) { Value = reserva.EstadoId });

            try
            {
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    reserva.ReservaId = Convert.ToInt32(result);
                }
            }
            catch (SqlException ex)
            {
                // Detectar el mensaje de solapamiento enviado desde el SP y lanzar excepción de dominio
                var message = ex.Message ?? string.Empty;
                if (message.IndexOf("solapad", StringComparison.OrdinalIgnoreCase) >= 0
                    || message.IndexOf("solapada", StringComparison.OrdinalIgnoreCase) >= 0
                    || message.IndexOf("Existe otra reserva", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new ReservaSolapadaException(message);
                }

                // Re-lanzar otras SqlException
                throw;
            }
        }

        // Actualiza usando el SP; captura SqlException similar
        public async Task UpdateAsync(Reserva reserva)
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Cadena de conexión no configurada. Configure 'ConnectionStrings:DefaultConnection'.");

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sp_GuardarReserva", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@ReservaId", SqlDbType.Int) { Value = reserva.ReservaId });
            cmd.Parameters.Add(new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = reserva.UsuarioId });
            cmd.Parameters.Add(new SqlParameter("@CanchaId", SqlDbType.Int) { Value = reserva.CanchaId });
            cmd.Parameters.Add(new SqlParameter("@FechaReserva", SqlDbType.DateTime2) { Value = reserva.FechaReserva });
            cmd.Parameters.Add(new SqlParameter("@HoraInicio", SqlDbType.Time) { Value = reserva.HoraInicio });
            cmd.Parameters.Add(new SqlParameter("@HoraFin", SqlDbType.Time) { Value = reserva.HoraFin });
            cmd.Parameters.Add(new SqlParameter("@EstadoId", SqlDbType.Int) { Value = reserva.EstadoId });

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                var message = ex.Message ?? string.Empty;
                if (message.IndexOf("solapad", StringComparison.OrdinalIgnoreCase) >= 0
                    || message.IndexOf("solapada", StringComparison.OrdinalIgnoreCase) >= 0
                    || message.IndexOf("Existe otra reserva", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new ReservaSolapadaException(message);
                }

                throw;
            }
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