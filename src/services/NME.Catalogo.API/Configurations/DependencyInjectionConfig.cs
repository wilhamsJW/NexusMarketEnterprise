using Microsoft.EntityFrameworkCore;
using NME.Catalogo.API.Data;
using NME.Catalogo.API.Data.Respository;
using NME.Catalogo.API.Models;
using NME.Catalogo.API.Services;

namespace NME.Catalogo.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyInjectionConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext com resiliência a falhas transitórias do SQL Server
            services.AddDbContext<CatalogoContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null)));

            // Repositories
            services.AddScoped<IProdutoRepository, ProdutoRepository>();

            // Application Services
            services.AddScoped<IProductAppService, ProductAppService>();

            return services;
        }
    }
}