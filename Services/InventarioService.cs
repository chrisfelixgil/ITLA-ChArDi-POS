using System.Net;
using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.Producto;
using Microsoft.EntityFrameworkCore;

namespace cahrdipos_system.Services;

/// <summary>
/// Implementación de <see cref="IInventarioService"/> que gestiona el stock de productos:
/// consulta del nivel actual y descuento atómico a partir de los detalles de una factura.
/// </summary>
public class InventarioService : IInventarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public InventarioService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Descuenta el stock de cada producto referenciado en los detalles de la factura,
    /// validando existencia y disponibilidad antes de persistir el cambio.
    /// Toda la operación se ejecuta dentro de una transacción atómica.
    /// </summary>
    /// <param name="facturaCreateDto">DTO con los detalles que indican qué productos y cantidades descontar.</param>
    /// <returns>
    /// <see cref="ResponseWrapper{T}"/> con <c>true</c> si el descuento fue exitoso (<c>200 OK</c>),
    /// o un error descriptivo si algún producto no existe o no tiene stock suficiente.
    /// </returns>
    public async Task<ResponseWrapper<bool>> DescontarStockAsync(FacturaCreateDto facturaCreateDto)
    {
        if (facturaCreateDto.Detalles is null || facturaCreateDto.Detalles.Count == 0)
            return new ResponseWrapper<bool>(
                "La factura debe incluir al menos un detalle.",
                HttpStatusCode.BadRequest);

        var cantidadPorProducto = facturaCreateDto.Detalles
            .GroupBy(d => d.ProductoId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var stock = await StockHelper.ValidarYDescontarAsync(_context, cantidadPorProducto);
        if (!stock.Ok)
        {
            await transaction.RollbackAsync();
            return new ResponseWrapper<bool>(stock.Mensaje!, stock.Status);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new ResponseWrapper<bool>(true, HttpStatusCode.OK);
    }

    /// <summary>
    /// Retorna el estado actual de un producto (incluyendo su stock disponible)
    /// a partir de su identificador.
    /// </summary>
    /// <param name="productoId">Identificador único del producto a consultar.</param>
    /// <returns>
    /// <see cref="ResponseWrapper{T}"/> con el <see cref="ProductoResponseDto"/> (<c>200 OK</c>),
    /// o <c>404 Not Found</c> si el producto no existe.
    /// </returns>
    public async Task<ResponseWrapper<ProductoResponseDto>> ObtenerStockActualAsync(int productoId)
    {
        var producto = await _context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productoId);

        if (producto is null)
            return new ResponseWrapper<ProductoResponseDto>("Producto no encontrado.", HttpStatusCode.NotFound);

        var response = _mapper.Map<ProductoResponseDto>(producto);
        return new ResponseWrapper<ProductoResponseDto>(response, HttpStatusCode.OK);
    }
}
