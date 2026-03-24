using System.ComponentModel.DataAnnotations;

namespace cahrdipos_system.Mapping.DTOs.FacturaDetalle
{
    /// <summary>
    /// Filtro por rango de fechas para reportes de ventas (facturas / detalle).
    /// </summary>
    public class ReporteVentasRequestDto
    {
        /// <summary>Inicio del rango (inclusive). Incluir hora si necesitas cortar el día.</summary>
        [Required]
        public DateTime FechaInicio { get; set; }

        /// <summary>Fin del rango (inclusive). Usar fin de día (23:59:59) si solo envías fecha.</summary>
        [Required]
        public DateTime FechaFin { get; set; }
    }
}
