using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.Producto;

namespace cahrdipos_system.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public InventarioService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Task<ResponseWrapper<bool>> DescontarStockAsync(FacturaCreateDto facturaCreateDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<ProductoResponseDto>> ObtenerStockActualAsync(int productoId)
        {
            throw new NotImplementedException();
        }
    }
}
