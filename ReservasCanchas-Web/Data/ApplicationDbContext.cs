using Microsoft.EntityFrameworkCore;
using System;
using ReservasCanchas_Web.Models;

namespace ReservasCanchas_Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Reserva> Reserva { get; set; } = null!;
        public DbSet<Cancha> Cancha { get; set; } = null!;
        public DbSet<Usuario> Usuario { get; set; } = null!;
        public DbSet<EstadoReserva> EstadoReserva { get; set; } = null!;
        public DbSet<MetodoPago> MetodoPago { get; set; } = null!;
        public DbSet<Pago> Pago { get; set; } = null!;
    }

}