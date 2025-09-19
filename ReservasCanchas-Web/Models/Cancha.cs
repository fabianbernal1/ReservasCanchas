using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasCanchas_Web.Models
{
    [Table("Canchas")]
    public class Cancha
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CanchaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Ubicacion { get; set; }   

        [StringLength(50)]
        public string? Tipo { get; set; }       

        [Required]
        [Column(TypeName = "decimal(10,2)")]   
        public decimal PrecioHora { get; set; }
    }
}
