namespace cahrdipos_system.Mapping.DTOs.FacturaDetalle
{
    /// <summary>
    /// Resultado del reporte de ventas: totales y, opcionalmente, líneas por factura/producto.
    /// </summary>
    public class ReporteVentasResponseDto
    {
        /// <summary>Cantidad de facturas en el rango.</summary>
        public int CantidadFacturas { get; set; }

        /// <summary>Suma de <c>facturas.total</c> en el rango.</summary>
        public decimal TotalVendido { get; set; }

        /// <summary>
        /// Detalle por línea (join factura + factura_detalles + producto). Vacío si el servicio solo llena resumen.
        /// </summary>
        public List<ReporteVentaDetalleLineaDto> Detalles { get; set; } = new();
    }

    /// <summary>
    /// Una fila del reporte detallado (equivalente al SELECT con facturas, detalles y productos).
    /// </summary>
    public class ReporteVentaDetalleLineaDto
    {
        public string NumeroFactura { get; set; } = null!;

        public DateTime FechaFactura { get; set; }

        public string NombreProducto { get; set; } = null!;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal SubtotalLinea { get; set; }
    }
}
