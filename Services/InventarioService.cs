using System.Net;
using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.Factura;
using cahrdipos_system.Mapping.DTOs.Producto;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ResponseWrapper<bool>> DescontarStockAsync(FacturaCreateDto facturaCreateDto)
        {
            if (facturaCreateDto.Detalles is null || facturaCreateDto.Detalles.Count == 0)
            {
                return new ResponseWrapper<bool>(
                    "La factura debe incluir al menos un detalle.",
                    HttpStatusCode.BadRequest);
            }

            var cantidadPorProducto = facturaCreateDto.Detalles
                .GroupBy(d => d.ProductoId)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));

            var productoIds = cantidadPorProducto.Keys.ToList();

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var productos = await _context.Productos
                .Where(p => productoIds.Contains(p.Id))
                .ToListAsync();

            if (productos.Count != productoIds.Count)
            {
                var encontrados = productos.Select(p => p.Id).ToHashSet();
                var faltantes = productoIds.Where(id => !encontrados.Contains(id)).ToList();
                await transaction.RollbackAsync();
                return new ResponseWrapper<bool>(
                    $"No se encontraron los productos con id: {string.Join(", ", faltantes)}.",
                    HttpStatusCode.NotFound);
            }

            foreach (var producto in productos)
            {
                var cantidad = cantidadPorProducto[producto.Id];
                if (producto.Stock < cantidad)
                {
                    await transaction.RollbackAsync();
                    return new ResponseWrapper<bool>(
                        $"Stock insuficiente para el producto \"{producto.Nombre}\" (id {producto.Id}). " +
                        $"Disponible: {producto.Stock}, requerido: {cantidad}.",
                        HttpStatusCode.BadRequest);
                }

                producto.Stock -= cantidad;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseWrapper<bool>(true, HttpStatusCode.OK);
        }

        public async Task<ResponseWrapper<ProductoResponseDto>> ObtenerStockActualAsync(int productoId)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productoId);

            if (producto is null)
            {
                return new ResponseWrapper<ProductoResponseDto>("Producto no encontrado.", HttpStatusCode.NotFound);
            }

            var response = _mapper.Map<ProductoResponseDto>(producto);
            return new ResponseWrapper<ProductoResponseDto>(response, HttpStatusCode.OK);
        }
    }
}
