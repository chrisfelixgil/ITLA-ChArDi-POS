using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.Producto;

namespace cahrdipos_system.Services
{
    public class ProductoService : IProductoService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductoService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<ResponseWrapper<ProductoResponseDto>> ActualizarProductoAsync(int id, ProductoUpdateDto productoUpdateDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<ProductoResponseDto>> CrearProductoAsync(ProductoCreateDto productoCreateDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<ProductoResponseDto>> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<IEnumerable<ProductoResponseDto>>> ObtenerProductosAsync()
        {
            throw new NotImplementedException();
        }
    }
}
