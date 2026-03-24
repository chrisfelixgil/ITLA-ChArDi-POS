namespace cahrdipos_system.Mapping.DTOs.Factura;

/// <summary>
/// Factura expuesta en respuestas API, con líneas de detalle.
/// </summary>
public class FacturaResponseDto
{
    public int Id { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Total { get; set; }

    public List<FacturaDetalleResponseDto> Detalles { get; set; } = new();
}

/// <summary>
/// Línea de detalle de una factura en respuestas API.
/// </summary>
public class FacturaDetalleResponseDto
{
    public int Id { get; set; }

    public int FacturaId { get; set; }

    public int ProductoId { get; set; }

    /// <summary>Nombre del producto (opcional; rellenar con join/include en el servicio).</summary>
    public string? ProductoNombre { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }
}
