using cahrdipos_system.Common;
using cahrdipos_system.Mapping.DTOs.Producto;

namespace cahrdipos_system.Interfaces
{
    public interface IProductoService
    {
        Task<ResponseWrapper<ProductoResponseDto>> CrearProductoAsync(ProductoCreateDto productoCreateDto);
        Task<ResponseWrapper<IEnumerable<ProductoResponseDto>>> ObtenerProductosAsync();
        Task<ResponseWrapper<ProductoResponseDto>> ActualizarProductoAsync(int id, ProductoUpdateDto productoUpdateDto);
        Task<ResponseWrapper<ProductoResponseDto>> ObtenerPorIdAsync(int id);
    }
}
