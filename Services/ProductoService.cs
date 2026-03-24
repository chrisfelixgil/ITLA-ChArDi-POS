using System.Net;
using AutoMapper;
using cahrdipos_system.Common;
using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping.DTOs.Producto;
using cahrdipos_system.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ResponseWrapper<ProductoResponseDto>> ActualizarProductoAsync(int id, ProductoUpdateDto productoUpdateDto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto is null)
            {
                return new ResponseWrapper<ProductoResponseDto>("Producto no encontrado.", HttpStatusCode.NotFound);
            }

            var codigoDuplicado = await _context.Productos
                .AnyAsync(p => p.Codigo == productoUpdateDto.Codigo && p.Id != id);
            if (codigoDuplicado)
            {
                return new ResponseWrapper<ProductoResponseDto>(
                    "Ya existe otro producto con el mismo código.",
                    HttpStatusCode.Conflict);
            }

            var activoAnterior = producto.Activo;
            _mapper.Map(productoUpdateDto, producto);
            if (!productoUpdateDto.Activo.HasValue)
            {
                producto.Activo = activoAnterior;
            }

            await _context.SaveChangesAsync();
            await _context.Entry(producto).ReloadAsync();

            var response = _mapper.Map<ProductoResponseDto>(producto);
            return new ResponseWrapper<ProductoResponseDto>(response, HttpStatusCode.OK);
        }

        public async Task<ResponseWrapper<ProductoResponseDto>> CrearProductoAsync(ProductoCreateDto productoCreateDto)
        {
            var codigoExiste = await _context.Productos
                .AnyAsync(p => p.Codigo == productoCreateDto.Codigo);
            if (codigoExiste)
            {
                return new ResponseWrapper<ProductoResponseDto>(
                    "Ya existe un producto con el mismo código.",
                    HttpStatusCode.Conflict);
            }

            var producto = _mapper.Map<Producto>(productoCreateDto);
            if (!productoCreateDto.Activo.HasValue)
            {
                producto.Activo = true;
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            await _context.Entry(producto).ReloadAsync();

            var response = _mapper.Map<ProductoResponseDto>(producto);
            return new ResponseWrapper<ProductoResponseDto>(response, HttpStatusCode.Created);
        }

        public async Task<ResponseWrapper<ProductoResponseDto>> ObtenerPorIdAsync(int id)
        {
            var producto = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (producto is null)
            {
                return new ResponseWrapper<ProductoResponseDto>("Producto no encontrado.", HttpStatusCode.NotFound);
            }

            var response = _mapper.Map<ProductoResponseDto>(producto);
            return new ResponseWrapper<ProductoResponseDto>(response, HttpStatusCode.OK);
        }

        public async Task<ResponseWrapper<IEnumerable<ProductoResponseDto>>> ObtenerProductosAsync()
        {
            var productos = await _context.Productos
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync();

            var response = _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);
            return new ResponseWrapper<IEnumerable<ProductoResponseDto>>(response, HttpStatusCode.OK);
        }
    }
}
