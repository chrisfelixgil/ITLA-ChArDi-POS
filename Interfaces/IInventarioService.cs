using cahrdipos_system.Common;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.Producto;

namespace cahrdipos_system.Interfaces
{
    public interface IInventarioService
    {
        Task<ResponseWrapper<ProductoResponseDto>> ObtenerStockActualAsync(int productoId);
        Task<ResponseWrapper<bool>> DescontarStockAsync(FacturaCreateDto facturaCreateDto);
    }
}
