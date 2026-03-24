using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.FacturaDetalle;

namespace cahrdipos_system.Services
{
    public class ReporteVentasService : IReporteVentasService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReporteVentasService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<ResponseWrapper<ReporteVentasResponseDto>> ObtenerReporteVentasAsync(ReporteVentasRequestDto reporteVentasRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}
