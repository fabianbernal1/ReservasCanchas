using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasCanchas_Web.Models
{
    [Table("Reservas")]
    public class Reserva
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ReservaId { get; set; }

        [Required]
        public int UsuarioId { get; set; }  

        [Required]
        public int CanchaId { get; set; }   

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaReserva { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan HoraFin { get; set; }

        [Required]
        public int EstadoId { get; set; }   

        
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }

        [ForeignKey(nameof(CanchaId))]
        public Cancha? Cancha { get; set; }

        [ForeignKey(nameof(EstadoId))]
        public EstadoReserva? Estado { get; set; }
    }
}
