using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NME.Identidade.API.Extensions;
using NME.Identidade.API.Services;

namespace NME.Identidade.API.Configuration
{
    public static class ApiConfig
    {
        private const string CorsPolicyName = "Total";

        public static IServiceCollection AddApiConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // IOptions<AppSettings>
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            // Controllers
            services.AddControllers();

            // Application Services
            services.AddScoped<JwtService>();
            services.AddScoped<AuthService>();

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(CorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            return app;
        }
    }
}
