namespace cahrdipos_system.Mapping.DTOs.Producto
{
    /// <summary>Producto devuelto por la API (lectura).</summary>
    public class ProductoResponseDto
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        /// <summary>Alineado al modelo EF (<c>TINYINT</c> con default en BD).</summary>
        public bool? Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}
