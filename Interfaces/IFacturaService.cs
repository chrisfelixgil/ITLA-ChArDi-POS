using cahrdipos_system.Common;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.FacturaDetalle;

namespace cahrdipos_system.Interfaces
{
    public interface IFacturaService
    {
        Task<ResponseWrapper<FacturaResponseDto>> CrearFacturaAsync(FacturaCreateDto facturaCreateDto);
        Task<ResponseWrapper<IEnumerable<FacturaResponseDto>>> ObtenerPorRangoFechasAsync(ReporteVentasRequestDto reporteVentasRequestDto);
        Task<ResponseWrapper<FacturaResponseDto>> CrearDetalleAsync(FacturaCreateDto facturaCreateDto);
    }
}
