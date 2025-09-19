using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasCanchas_Web.Models
{
    [Table("EstadosReserva")]
    public class EstadoReserva
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int EstadoId { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreEstado { get; set; } = string.Empty; 
    }
}
