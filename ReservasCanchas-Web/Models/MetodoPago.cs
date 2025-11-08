using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasCanchas_Web.Models
{
    [Table("MetodosPago")]
    public class MetodoPago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MetodoId { get; set; }
        [Required]
        [StringLength(50)]
        public string NombreMetodo { get; set; } = string.Empty;
    }
}
