using System.Net;
using cahrdipos_system.Context;
using cahrdipos_system.Models;
using Microsoft.EntityFrameworkCore;

namespace cahrdipos_system.Services;

/// <summary>
/// Resultado de <see cref="StockHelper.ValidarYDescontarAsync"/>.
/// Cuando <see cref="Ok"/> es true, <see cref="Productos"/> contiene las entidades
/// ya modificadas (stock decrementado) pero aún no persistidas.
/// </summary>
internal sealed record StockOperacionResultado(
    bool Ok,
    string? Mensaje,
    HttpStatusCode Status,
    IReadOnlyList<Producto> Productos);

/// <summary>
/// Lógica compartida de stock reutilizable dentro de la capa de servicios.
/// No gestiona transacciones ni llama a SaveChanges; esa responsabilidad
/// recae en el servicio que invoca este helper.
/// </summary>
internal static class StockHelper
{
    /// <summary>
    /// Carga los productos indicados en <paramref name="cantidadPorProducto"/>,
    /// valida que todos existan y que haya stock suficiente para cada uno,
    /// y decremente el stock en las entidades rastreadas por EF.
    /// </summary>
    internal static async Task<StockOperacionResultado> ValidarYDescontarAsync(
        ApplicationDbContext context,
        Dictionary<int, int> cantidadPorProducto)
    {
        var productoIds = cantidadPorProducto.Keys.ToList();

        var productos = await context.Productos
            .Where(p => productoIds.Contains(p.Id))
            .ToListAsync();

        if (productos.Count != productoIds.Count)
        {
            var encontrados = productos.Select(p => p.Id).ToHashSet();
            var faltantes = productoIds.Where(id => !encontrados.Contains(id));
            return Fallo(
                $"No se encontraron los productos con id: {string.Join(", ", faltantes)}.",
                HttpStatusCode.NotFound);
        }

        foreach (var producto in productos)
        {
            var cantidad = cantidadPorProducto[producto.Id];
            if (producto.Stock < cantidad)
                return Fallo(
                    $"Stock insuficiente para el producto \"{producto.Nombre}\" (id {producto.Id}). " +
                    $"Disponible: {producto.Stock}, requerido: {cantidad}.",
                    HttpStatusCode.BadRequest);
        }

        foreach (var producto in productos)
            producto.Stock -= cantidadPorProducto[producto.Id];

        return new StockOperacionResultado(true, null, HttpStatusCode.OK, productos);
    }

    private static StockOperacionResultado Fallo(string mensaje, HttpStatusCode status) =>
        new(false, mensaje, status, []);
}
