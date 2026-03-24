using System.ComponentModel.DataAnnotations;

namespace cahrdipos_system.Mapping.DTOs.Producto
{
    /// <summary>Actualización de producto (mismo shape que creación; el <c>id</c> va en la ruta).</summary>
    public class ProductoUpdateDto
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

        public bool? Activo { get; set; }
    }
}
