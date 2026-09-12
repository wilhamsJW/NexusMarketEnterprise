using NME.WebApp.MVC.Configuration;

namespace NME.WebApp.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Registra o padrão Options Pattern no container de DI: lê a seção "AppSettings" do appsettings.json 
            // e disponibiliza a interface IOptions<AppSettings> para ser injetada em outros pontos do sistema 
            // (usada no DependencyInjectionConfig.cs via IServiceProvider para pegar a propriedade CatalogoUrl/AutenticacaoUrl).
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddIdentityConfiguration();

            // Centraliza a injeção de dependências dos HttpClients e Serviços
            builder.Services.RegisterServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
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