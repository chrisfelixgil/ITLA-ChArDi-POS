using cahrdipos_system.Common;
using cahrdipos_system.Mapping.DTOs.FacturaDetalle;

namespace cahrdipos_system.Interfaces
{
    public interface IReporteVentasService
    {
        Task<ResponseWrapper<ReporteVentasResponseDto>> ObtenerReporteVentasAsync(ReporteVentasRequestDto reporteVentasRequestDto);
    }
}
