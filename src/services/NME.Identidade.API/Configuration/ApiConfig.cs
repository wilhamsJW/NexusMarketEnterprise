using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NME.Identidade.API.Extensions;
using NME.Identidade.API.Services;
using System.Threading.RateLimiting;

namespace NME.Identidade.API.Configuration
{
    public static class ApiConfig
    {
        private const string CorsPolicyName = "Total";

        /// <summary>
        /// Nome padronizado da política de Rate Limiting de Janela Fixa.
        /// O uso de constante exposta (const string) elimina o risco de erros de digitação ("magic strings") 
        /// ao referenciar a política nas Controllers via o atributo [EnableRateLimiting(RateLimiterExtensions.FixedWindowPolicy)].
        /// </summary>
        public const string FixedWindowPolicy = "FixedWindow";

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

            // Rate Limiting: janela fixa por IP para proteger contra brute-force/DDoS
            services.AddRateLimiter(options =>
            {
                // Rejeição uniforme com 429 Too Many Requests
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(FixedWindowPolicy, httpContext =>
                {
                    // Particionamento pelo IP remoto — cada origem tem sua própria janela
                    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()
                                    ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            // Número máximo de requisições permitidas na janela (ideal para login/autenticação)
                            PermitLimit = 5,

                            // Duração da janela de tempo em que a cota é calculada (1 minuto)
                            Window = TimeSpan.FromMinutes(1),

                            // Tamanho da fila de espera. Com 0, a 6ª requisição é rejeitada imediatamente com HTTP 429
                            QueueLimit = 0,

                            // Zera o contador e libera 5 novas fichas automaticamente ao fim do intervalo de 1 minuto
                            AutoReplenishment = true
                        });
                });
            });

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            // Em Development a API roda apenas em HTTP puro.
            // Redirecionar aqui causaria 307 e quebraria o POST vindo do MVC.
            if (!env.IsDevelopment())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseCors(CorsPolicyName);

            // Rate limiter aplicado após o roteamento (necessário para políticas por endpoint/atributo)
            // e antes da autenticação, para bloquear floods sem consumir CPU validando JWT.
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            return app;
        }
    }
}
