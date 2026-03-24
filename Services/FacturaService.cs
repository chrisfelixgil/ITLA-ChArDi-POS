using System.Net;
using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.FacturaDetalle;
using cahrdipos_system.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ResponseWrapper<FacturaResponseDto>> CrearDetalleAsync(FacturaCreateDto facturaCreateDto)
        {
            if (facturaCreateDto.Detalles is null || facturaCreateDto.Detalles.Count == 0)
            {
                return new ResponseWrapper<FacturaResponseDto>(
                    "La factura debe incluir al menos un detalle.",
                    HttpStatusCode.BadRequest);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var factura = await _context.Facturas
                .FirstOrDefaultAsync(f => f.NumeroFactura == facturaCreateDto.NumeroFactura);

            if (factura is null)
            {
                await transaction.RollbackAsync();
                return new ResponseWrapper<FacturaResponseDto>("Factura no encontrada.", HttpStatusCode.NotFound);
            }

            var cantidadPorProducto = facturaCreateDto.Detalles
                .GroupBy(d => d.ProductoId)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));

            var productoIds = cantidadPorProducto.Keys.ToList();

            var productos = await _context.Productos
                .Where(p => productoIds.Contains(p.Id))
                .ToListAsync();

            if (productos.Count != productoIds.Count)
            {
                var encontrados = productos.Select(p => p.Id).ToHashSet();
                var faltantes = productoIds.Where(id => !encontrados.Contains(id)).ToList();
                await transaction.RollbackAsync();
                return new ResponseWrapper<FacturaResponseDto>(
                    $"No se encontraron los productos con id: {string.Join(", ", faltantes)}.",
                    HttpStatusCode.NotFound);
            }

            foreach (var producto in productos)
            {
                var cantidad = cantidadPorProducto[producto.Id];
                if (producto.Stock < cantidad)
                {
                    await transaction.RollbackAsync();
                    return new ResponseWrapper<FacturaResponseDto>(
                        $"Stock insuficiente para el producto \"{producto.Nombre}\" (id {producto.Id}). " +
                        $"Disponible: {producto.Stock}, requerido: {cantidad}.",
                        HttpStatusCode.BadRequest);
                }
            }

            decimal sumaNuevasLineas = 0;

            foreach (var linea in facturaCreateDto.Detalles)
            {
                var producto = productos.Single(p => p.Id == linea.ProductoId);
                var precioUnitario = linea.PrecioUnitario ?? producto.Precio;
                var subtotalLinea = precioUnitario * linea.Cantidad;
                sumaNuevasLineas += subtotalLinea;

                var detalle = _mapper.Map<FacturaDetalle>(linea);
                detalle.FacturaId = factura.Id;
                detalle.PrecioUnitario = precioUnitario;
                detalle.Subtotal = subtotalLinea;
                _context.FacturaDetalles.Add(detalle);
            }

            foreach (var producto in productos)
            {
                producto.Stock -= cantidadPorProducto[producto.Id];
            }

            factura.Subtotal += sumaNuevasLineas;
            factura.Total = factura.Subtotal;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var facturaRespuesta = await _context.Facturas
                .AsNoTracking()
                .Include(f => f.FacturaDetalles)
                .ThenInclude(fd => fd.Producto)
                .FirstAsync(f => f.Id == factura.Id);

            var response = _mapper.Map<FacturaResponseDto>(facturaRespuesta);
            return new ResponseWrapper<FacturaResponseDto>(response, HttpStatusCode.OK);
        }

        public async Task<ResponseWrapper<FacturaResponseDto>> CrearFacturaAsync(FacturaCreateDto facturaCreateDto)
        {
            if (facturaCreateDto.Detalles is null || facturaCreateDto.Detalles.Count == 0)
            {
                return new ResponseWrapper<FacturaResponseDto>(
                    "La factura debe incluir al menos un detalle.",
                    HttpStatusCode.BadRequest);
            }

            var numeroDuplicado = await _context.Facturas
                .AnyAsync(f => f.NumeroFactura == facturaCreateDto.NumeroFactura);
            if (numeroDuplicado)
            {
                return new ResponseWrapper<FacturaResponseDto>(
                    "Ya existe una factura con el mismo número.",
                    HttpStatusCode.Conflict);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var cantidadPorProducto = facturaCreateDto.Detalles
                .GroupBy(d => d.ProductoId)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));

            var productoIds = cantidadPorProducto.Keys.ToList();

            var productos = await _context.Productos
                .Where(p => productoIds.Contains(p.Id))
                .ToListAsync();

            if (productos.Count != productoIds.Count)
            {
                var encontrados = productos.Select(p => p.Id).ToHashSet();
                var faltantes = productoIds.Where(id => !encontrados.Contains(id)).ToList();
                await transaction.RollbackAsync();
                return new ResponseWrapper<FacturaResponseDto>(
                    $"No se encontraron los productos con id: {string.Join(", ", faltantes)}.",
                    HttpStatusCode.NotFound);
            }

            foreach (var producto in productos)
            {
                var cantidad = cantidadPorProducto[producto.Id];
                if (producto.Stock < cantidad)
                {
                    await transaction.RollbackAsync();
                    return new ResponseWrapper<FacturaResponseDto>(
                        $"Stock insuficiente para el producto \"{producto.Nombre}\" (id {producto.Id}). " +
                        $"Disponible: {producto.Stock}, requerido: {cantidad}.",
                        HttpStatusCode.BadRequest);
                }
            }

            decimal sumaLineas = 0;
            var factura = _mapper.Map<Factura>(facturaCreateDto);

            foreach (var linea in facturaCreateDto.Detalles)
            {
                var producto = productos.Single(p => p.Id == linea.ProductoId);
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

            foreach (var producto in productos)
            {
                producto.Stock -= cantidadPorProducto[producto.Id];
            }

            _context.Facturas.Add(factura);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var facturaRespuesta = await _context.Facturas
                .AsNoTracking()
                .Include(f => f.FacturaDetalles)
                .ThenInclude(fd => fd.Producto)
                .FirstAsync(f => f.Id == factura.Id);

            var response = _mapper.Map<FacturaResponseDto>(facturaRespuesta);
            return new ResponseWrapper<FacturaResponseDto>(response, HttpStatusCode.Created);
        }

        public async Task<ResponseWrapper<IEnumerable<FacturaResponseDto>>> ObtenerPorRangoFechasAsync(ReporteVentasRequestDto reporteVentasRequestDto)
        {
            if (reporteVentasRequestDto is null)
            {
                return new ResponseWrapper<IEnumerable<FacturaResponseDto>>(
                    "Datos de reporte requeridos.",
                    HttpStatusCode.BadRequest);
            }

            if (reporteVentasRequestDto.FechaInicio > reporteVentasRequestDto.FechaFin)
            {
                return new ResponseWrapper<IEnumerable<FacturaResponseDto>>(
                    "La fecha de inicio no puede ser mayor que la fecha fin.",
                    HttpStatusCode.BadRequest);
            }

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
    }
}
