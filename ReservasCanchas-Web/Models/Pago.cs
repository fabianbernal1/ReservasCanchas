using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasCanchas_Web.Models
{
    [Table("Pagos")]
    public class Pago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int PagoId { get; set; }

        [Required]
        public int ReservaId { get; set; } 

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Required]
        public int MetodoId { get; set; } 

        
        [ForeignKey(nameof(ReservaId))]
        public Reserva? Reserva { get; set; }

        [ForeignKey(nameof(MetodoId))]
        public MetodoPago? MetodoPago { get; set; }
    }
}
