using System.ComponentModel.DataAnnotations;

namespace cahrdipos_system.Mapping.DTOs.Factura;

/// <summary>
/// Datos para registrar una factura con sus detalles. Subtotal y total suelen calcularse en el servicio a partir de los detalles.
/// </summary>
public class FacturaCreateDto
{
    [Required]
    [MaxLength(30)]
    public string NumeroFactura { get; set; } = null!;

    /// <summary>
    /// Opcional. Si es null, se usa la fecha por defecto del servidor / base de datos.
    /// </summary>
    public DateTime? Fecha { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "La factura debe incluir al menos un detalle.")]
    public List<FacturaDetalleCreateDto> Detalles { get; set; } = new();
}
