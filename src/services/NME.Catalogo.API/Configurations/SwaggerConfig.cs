using Microsoft.OpenApi.Models;

namespace NME.Catalogo.API.Configurations
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NME Catálogo API",
                    Version = "v1",
                    Description = "API de Catálogo do Nexus Market Enterprise",
                    Contact = new OpenApiContact
                    {
                        Name = "Nexus Market Enterprise",
                        Email = "contato@nexusmarket.com"
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NME Catálogo API v1");
                // Swagger UI opens automatically at root
                c.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}