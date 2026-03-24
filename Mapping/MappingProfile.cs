using AutoMapper;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.Producto;
using cahrdipos_system.Models;

namespace cahrdipos_system.Mapping;

/// <summary>
/// Perfiles AutoMapper entre entidades EF (scaffold) y DTOs de API.
/// Los reportes (<c>ReporteVentasResponseDto</c>) suelen armarse en el servicio con proyecciones LINQ.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        MapProducto();
        MapFacturaYDetalle();
    }

    private void MapProducto()
    {
        CreateMap<Producto, ProductoResponseDto>();

        CreateMap<ProductoCreateDto, Producto>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.FechaCreacion, o => o.Ignore())
            .ForMember(d => d.FechaActualizacion, o => o.Ignore())
            .ForMember(d => d.FacturaDetalles, o => o.Ignore());

        CreateMap<ProductoUpdateDto, Producto>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.FechaCreacion, o => o.Ignore())
            .ForMember(d => d.FechaActualizacion, o => o.Ignore())
            .ForMember(d => d.FacturaDetalles, o => o.Ignore());
    }

    private void MapFacturaYDetalle()
    {
        CreateMap<Factura, FacturaResponseDto>()
            .ForMember(d => d.Detalles, o => o.MapFrom(s => s.FacturaDetalles));

        // Cabecera: subtotal/total y líneas los calcula y persiste el servicio.
        CreateMap<FacturaCreateDto, Factura>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Fecha, o => o.MapFrom(s => s.Fecha ?? DateTime.UtcNow))
            .ForMember(d => d.Subtotal, o => o.Ignore())
            .ForMember(d => d.Total, o => o.Ignore())
            .ForMember(d => d.FacturaDetalles, o => o.Ignore())
            .ForSourceMember(s => s.Detalles, o => o.DoNotValidate());

        CreateMap<FacturaDetalle, FacturaDetalleResponseDto>()
            .ForMember(d => d.ProductoNombre, o => o.MapFrom(s => s.Producto != null ? s.Producto.Nombre : null));

        CreateMap<FacturaDetalleCreateDto, FacturaDetalle>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.FacturaId, o => o.Ignore())
            .ForMember(d => d.Subtotal, o => o.Ignore())
            .ForMember(d => d.PrecioUnitario, o => o.MapFrom(s => s.PrecioUnitario ?? 0))
            .ForMember(d => d.Factura, o => o.Ignore())
            .ForMember(d => d.Producto, o => o.Ignore());
    }
}
