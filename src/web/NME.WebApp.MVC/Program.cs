using NME.WebApp.MVC.Configuration;
using NME.WebApp.MVC.Interfaces;
using NME.WebApp.MVC.Providers;
using NME.WebApp.MVC.Services;
using Polly;

namespace NME.WebApp.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddIdentityConfiguration();

            // Necessário para o SignInAsync dentro do AutenticacaoService
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUser, AspNetUser>();

            // HttpClient tipado apontando para a API de Identidade
            builder.Services.AddHttpClient<IAutenticacaoService, AutenticacaoService>(client =>
            {
                var identidadeUrl = builder.Configuration["IdentidadeUrl"]
                    ?? throw new InvalidOperationException("Configuração 'IdentidadeUrl' não definida.");

                // Garante a barra final: sem ela o último segmento da base é descartado
                if (!identidadeUrl.EndsWith('/')) identidadeUrl += "/";

                client.BaseAddress = new Uri(identidadeUrl);

                // Timeout do HttpClient acima do total da pipeline para não competir com o Polly
                client.Timeout = TimeSpan.FromSeconds(60);
            })
            // Pipeline padrão: rate limiter -> total timeout -> retry -> circuit breaker -> attempt timeout
            .AddStandardResilienceHandler(options =>
            {
                // Teto global da requisição, cobrindo todas as tentativas e backoffs
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);

                // Timeout individual por tentativa
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);

                // 3 retentativas com backoff exponencial e jitter (evita thundering herd)
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromSeconds(2);
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.Retry.UseJitter = true;

                // Disjuntor abre com 50% de falhas 5xx na janela, protegendo a API sob estresse
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = 5;
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityConfiguration();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
