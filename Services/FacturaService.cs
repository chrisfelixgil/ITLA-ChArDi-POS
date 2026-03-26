using System.Net;
using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.FacturaDetalle;
using cahrdipos_system.Models;
using Microsoft.EntityFrameworkCore;

namespace cahrdipos_system.Services;

/// <summary>
/// Implementación de <see cref="IFacturaService"/> que gestiona la creación de facturas
/// y sus líneas de detalle, incluyendo validación de stock y cálculo de totales.
/// </summary>
public class FacturaService : IFacturaService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public FacturaService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Crea una factura nueva junto con sus líneas de detalle.
    /// Valida existencia y stock de cada producto dentro de una transacción atómica
    /// y descuenta el stock al confirmar.
    /// </summary>
    /// <param name="facturaCreateDto">Datos de cabecera y detalles de la factura a crear.</param>
    /// <returns>
    /// <see cref="ResponseWrapper{T}"/> con la factura creada (<c>201 Created</c>),
    /// o un error descriptivo en caso de validación fallida.
    /// </returns>
    public async Task<ResponseWrapper<FacturaResponseDto>> CrearFacturaAsync(FacturaCreateDto facturaCreateDto)
    {
        if (facturaCreateDto.Detalles is null || facturaCreateDto.Detalles.Count == 0)
            return new ResponseWrapper<FacturaResponseDto>(
                "La factura debe incluir al menos un detalle.",
                HttpStatusCode.BadRequest);

        var numeroDuplicado = await _context.Facturas
            .AnyAsync(f => f.NumeroFactura == facturaCreateDto.NumeroFactura);
        if (numeroDuplicado)
            return new ResponseWrapper<FacturaResponseDto>(
                "Ya existe una factura con el mismo número.",
                HttpStatusCode.Conflict);

        var cantidadPorProducto = BuildCantidadPorProducto(facturaCreateDto);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var stock = await StockHelper.ValidarYDescontarAsync(_context, cantidadPorProducto);
        if (!stock.Ok)
        {
            await transaction.RollbackAsync();
            return new ResponseWrapper<FacturaResponseDto>(stock.Mensaje!, stock.Status);
        }

        var factura = _mapper.Map<Factura>(facturaCreateDto);
        decimal sumaLineas = 0;

        foreach (var linea in facturaCreateDto.Detalles)
        {
            var producto = stock.Productos.Single(p => p.Id == linea.ProductoId);
            var precioUnitario = linea.PrecioUnitario ?? producto.Precio;
            var subtotalLinea = precioUnitario * linea.Cantidad;
            sumaLineas += subtotalLinea;

            var detalle = _mapper.Map<FacturaDetalle>(linea);
            detalle.PrecioUnitario = precioUnitario;
            detalle.Subtotal = subtotalLinea;
            factura.FacturaDetalles.Add(detalle);
        }

        factura.Subtotal = sumaLineas;
        factura.Total = sumaLineas;

        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new ResponseWrapper<FacturaResponseDto>(
            await CargarFacturaCompletaAsync(factura.Id),
            HttpStatusCode.Created);
    }

    /// <summary>
    /// Agrega nuevas líneas de detalle a una factura existente identificada por
    /// <see cref="FacturaCreateDto.NumeroFactura"/>.
    /// Valida stock y lo descuenta atómicamente; actualiza subtotal y total de la cabecera.
    /// </summary>
    /// <param name="facturaCreateDto">
    /// Número de factura destino y las nuevas líneas a incorporar.
    /// </param>
    /// <returns>
    /// <see cref="ResponseWrapper{T}"/> con la factura actualizada (<c>200 OK</c>),
    /// o un error si la factura no existe o hay stock insuficiente.
    /// </returns>
    public async Task<ResponseWrapper<FacturaResponseDto>> CrearDetalleAsync(FacturaCreateDto facturaCreateDto)
    {
        if (facturaCreateDto.Detalles is null || facturaCreateDto.Detalles.Count == 0)
            return new ResponseWrapper<FacturaResponseDto>(
                "La factura debe incluir al menos un detalle.",
                HttpStatusCode.BadRequest);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var factura = await _context.Facturas
            .FirstOrDefaultAsync(f => f.NumeroFactura == facturaCreateDto.NumeroFactura);

        if (factura is null)
        {
            await transaction.RollbackAsync();
            return new ResponseWrapper<FacturaResponseDto>("Factura no encontrada.", HttpStatusCode.NotFound);
        }

        var cantidadPorProducto = BuildCantidadPorProducto(facturaCreateDto);

        var stock = await StockHelper.ValidarYDescontarAsync(_context, cantidadPorProducto);
        if (!stock.Ok)
        {
            await transaction.RollbackAsync();
            return new ResponseWrapper<FacturaResponseDto>(stock.Mensaje!, stock.Status);
        }

        decimal sumaNuevasLineas = 0;

        foreach (var linea in facturaCreateDto.Detalles)
        {
            var producto = stock.Productos.Single(p => p.Id == linea.ProductoId);
            var precioUnitario = linea.PrecioUnitario ?? producto.Precio;
            var subtotalLinea = precioUnitario * linea.Cantidad;
            sumaNuevasLineas += subtotalLinea;

            var detalle = _mapper.Map<FacturaDetalle>(linea);
            detalle.FacturaId = factura.Id;
            detalle.PrecioUnitario = precioUnitario;
            detalle.Subtotal = subtotalLinea;
            _context.FacturaDetalles.Add(detalle);
        }

        factura.Subtotal += sumaNuevasLineas;
        factura.Total = factura.Subtotal;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new ResponseWrapper<FacturaResponseDto>(
            await CargarFacturaCompletaAsync(factura.Id),
            HttpStatusCode.OK);
    }

    /// <summary>
    /// Retorna todas las facturas (con sus detalles y productos) cuya fecha
    /// se encuentre dentro del rango especificado, ordenadas de más reciente a más antigua.
    /// </summary>
    /// <param name="reporteVentasRequestDto">Rango de fechas de inicio y fin, ambos inclusive.</param>
    /// <returns>
    /// <see cref="ResponseWrapper{T}"/> con la colección de facturas del período (<c>200 OK</c>),
    /// o un error de validación si el rango es inválido.
    /// </returns>
    public async Task<ResponseWrapper<IEnumerable<FacturaResponseDto>>> ObtenerPorRangoFechasAsync(
        ReporteVentasRequestDto reporteVentasRequestDto)
    {
        if (reporteVentasRequestDto is null)
            return new ResponseWrapper<IEnumerable<FacturaResponseDto>>(
                "Datos de reporte requeridos.",
                HttpStatusCode.BadRequest);

        if (reporteVentasRequestDto.FechaInicio > reporteVentasRequestDto.FechaFin)
            return new ResponseWrapper<IEnumerable<FacturaResponseDto>>(
                "La fecha de inicio no puede ser mayor que la fecha fin.",
                HttpStatusCode.BadRequest);

        var facturas = await _context.Facturas
            .AsNoTracking()
            .Include(f => f.FacturaDetalles)
            .ThenInclude(fd => fd.Producto)
            .Where(f => f.Fecha >= reporteVentasRequestDto.FechaInicio &&
                        f.Fecha <= reporteVentasRequestDto.FechaFin)
            .OrderByDescending(f => f.Fecha)
            .ToListAsync();

        var response = _mapper.Map<IEnumerable<FacturaResponseDto>>(facturas);
        return new ResponseWrapper<IEnumerable<FacturaResponseDto>>(response, HttpStatusCode.OK);
    }

    // ── helpers privados ────────────────────────────────────────────────────

    /// <summary>
    /// Agrega las cantidades de cada producto solicitado, agrupando líneas con el mismo
    /// <c>ProductoId</c> para obtener el total requerido por producto en un solo paso.
    /// </summary>
    private static Dictionary<int, int> BuildCantidadPorProducto(FacturaCreateDto dto) =>
        dto.Detalles
            .GroupBy(d => d.ProductoId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));

    /// <summary>
    /// Recarga la factura desde la BD en modo <c>AsNoTracking</c> con sus líneas
    /// y productos incluidos, lista para proyectar al DTO de respuesta.
    /// </summary>
    /// <param name="facturaId">Identificador de la factura a recuperar.</param>
    private async Task<FacturaResponseDto> CargarFacturaCompletaAsync(int facturaId)
    {
        var factura = await _context.Facturas
            .AsNoTracking()
            .Include(f => f.FacturaDetalles)
            .ThenInclude(fd => fd.Producto)
            .FirstAsync(f => f.Id == facturaId);

        return _mapper.Map<FacturaResponseDto>(factura);
    }
}
