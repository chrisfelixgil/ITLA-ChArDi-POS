using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.FacturaDetalle;

namespace cahrdipos_system.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FacturaService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<ResponseWrapper<FacturaResponseDto>> CrearDetalleAsync(FacturaCreateDto facturaCreateDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<FacturaResponseDto>> CrearFacturaAsync(FacturaCreateDto facturaCreateDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<IEnumerable<FacturaResponseDto>>> ObtenerPorRangoFechasAsync(ReporteVentasRequestDto reporteVentasRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}
