using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DALL.DBContext;
using SistemaVenta.DALL.Implemtancion;
using SistemaVenta.DALL.Interfaces;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.BLL.Implemtancion;
using System.Runtime.CompilerServices;
using SistemaVenta.BLL.Implementacion;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContext<DbventaContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("cadendaSQL"));
            });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVentaRepository, VentaRepository>();
            services.AddScoped<ICorreoServices, CorreoService>();
            services.AddScoped<IFireBaseServices, FireBaseService>();
            services.AddScoped<IUtilidadesServices, UtilidadesServices>();
            services.AddScoped<IUsuarioServices, UsuarioServices>();
            services.AddScoped<IRolServices, RolServices>();
            services.AddScoped<INegocioService, NegocioService>();
            services.AddScoped<ICategoriaServices, ICategoriaServices>();
        }
    }
}