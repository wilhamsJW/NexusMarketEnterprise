using NME.WebApp.MVC.Configuration;
using NME.WebApp.MVC.Interfaces;
using NME.WebApp.MVC.Providers;
using NME.WebApp.MVC.Services;

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

            // HttpClient tipado apontando para a API de Identidade
            builder.Services.AddHttpClient<IAutenticacaoService, AutenticacaoService>(client =>
            {
                var identidadeUrl = builder.Configuration["IdentidadeUrl"]
                    ?? throw new InvalidOperationException("Configuração 'IdentidadeUrl' não definida.");

                // Garante a barra final: sem ela o último segmento da base é descartado
                if (!identidadeUrl.EndsWith('/')) identidadeUrl += "/";

                client.BaseAddress = new Uri(identidadeUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Provedor dos dados de identidade do usuario no contexto HTTP atual
            // Resolve claims, email e token JWT da requisicao
            builder.Services.AddScoped<IUser, AspNetUser>();

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
