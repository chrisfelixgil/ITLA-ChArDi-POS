using System.ComponentModel.DataAnnotations;

namespace cahrdipos_system.Mapping.DTOs.Producto
{
    /// <summary>Alta de producto. Fechas y <c>id</c> los asigna la base de datos / servicio.</summary>
    public class ProductoCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Codigo { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = null!;

        [MaxLength(255)]
        public string? Descripcion { get; set; }

        [Range(typeof(decimal), "0", "9999999999")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        /// <summary>Si es null, puede aplicarse el default de la BD (activo = 1).</summary>
        public bool? Activo { get; set; }
    }
}
