using NME.WebApp.MVC.Configuration;
using NME.WebApp.MVC.Interfaces;
using NME.WebApp.MVC.Providers;
using NME.WebApp.MVC.Services;
using Polly;
using NME.Core;

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

            // HttpClient tipado apontando para a API de Identidade.
            // Registra o contrato IAutenticacaoService e sua implementação AutenticacaoService no container de DI.
            builder.Services.AddHttpClient<IAutenticacaoService, AutenticacaoService>(client =>
            {
                // Recupera a URL base configurada no appsettings.json
                var identidadeUrl = builder.Configuration["IdentidadeUrl"]
                    ?? throw new InvalidOperationException("Configuração 'IdentidadeUrl' não definida.");

                // Garante a barra final na URL para evitar que a API descarte o último segmento de rota
                if (!identidadeUrl.EndsWith('/')) identidadeUrl += "/";

                // Define o endereço base para todas as chamadas feitas por este AutenticacaoService
                client.BaseAddress = new Uri(identidadeUrl);

                // Timeout bruto da instância do HttpClient (sockets de SO). 
                // Deve ser SEMPRE MAIOR que o TotalRequestTimeout do Polly para que o Polly controle o tempo, e não o driver HTTP.
                client.Timeout = TimeSpan.FromSeconds(60);
            }).AddStandardResilienceHandler();

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
