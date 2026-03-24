using System.ComponentModel.DataAnnotations;

namespace cahrdipos_system.Mapping.DTOs.Factura;

/// <summary>
/// Línea de detalle al crear una factura (sin <c>factura_id</c>; lo asigna el servicio).
/// </summary>
public class FacturaDetalleCreateDto
{
    [Required]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario al momento de la venta (snapshot). Si es null, el servicio puede tomarlo del catálogo.
    /// </summary>
    public decimal? PrecioUnitario { get; set; }
}
