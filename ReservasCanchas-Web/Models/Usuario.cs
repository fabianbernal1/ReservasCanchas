using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasCanchas_Web.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("Prim_apellido")]
        [StringLength(100)]
        public string PrimerApellido { get; set; } = string.Empty;
        
        [Column("Seg_apellido")]
        [StringLength(100)]
        public string? SegundoApellido { get; set; }

        [Required]
        [StringLength(12)]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [StringLength(20)]
        [Phone]
        public string? Telefono { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
