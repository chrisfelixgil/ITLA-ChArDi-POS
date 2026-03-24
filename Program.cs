using cahrdipos_system.Context;
using cahrdipos_system.Interfaces;
using cahrdipos_system.Mapping;
using cahrdipos_system.Services;
using Microsoft.EntityFrameworkCore;

namespace cahrdipos_system
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Servicios
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Conexi�n a la base de datos
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


            builder.Services.AddSwaggerGen();

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            //Registrar los servicios en el contenedor de inyeccion de dependencias
            builder.Services.AddScoped<IFacturaService, FacturaService>();
            builder.Services.AddScoped<IInventarioService, InventarioService>();
            builder.Services.AddScoped<IProductoService, ProductoService>();
            builder.Services.AddScoped<IReporteVentasService, ReporteVentasService>();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();       
                app.UseSwaggerUI();     
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
