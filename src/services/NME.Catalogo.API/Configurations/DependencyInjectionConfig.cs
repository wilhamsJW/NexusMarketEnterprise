using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NME.Catalogo.API.Data;
using NME.Catalogo.API.Data.Respository;
using NME.Catalogo.API.Models;
using NME.Catalogo.API.Service;
using NME.Catalogo.API.Services;
using System.Text;

namespace NME.Catalogo.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyInjectionConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext com resiliência a falhas transitórias
            services.AddDbContext<CatalogoContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null)));

            // Application Services
            services.AddScoped<IProductAppService, ProductAppService>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            return services;
        }

        public static IServiceCollection AddJwtConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            var secret = configuration["AppSettings:Secret"]
                ?? throw new InvalidOperationException("'AppSettings:Secret' não configurado.");

            var emissor = configuration["AppSettings:Emissor"];
            var validoEm = configuration["AppSettings:ValidoEm"];

            var key = Encoding.UTF8.GetBytes(secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions =>
            {
                // HTTP puro em Development: nunca exigir metadata via HTTPS
                bearerOptions.RequireHttpsMetadata = false;
                bearerOptions.SaveToken = true;
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = validoEm,
                    ValidIssuer = emissor,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}