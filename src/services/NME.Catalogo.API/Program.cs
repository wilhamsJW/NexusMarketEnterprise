
using Microsoft.EntityFrameworkCore;
using NME.Catalogo.API.Data;

namespace NME.Catalogo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. REGISTRO DO DBCONTEXT NO CONTAINER DE INJEÇÃO DE DEPENDÊNCIA
            // Lógica: Lê a "DefaultConnection" do appsettings.json e injeta
            // o 'DbContextOptions<CatalogoContext>' via construtor do CatalogoContext.
            builder.Services.AddDbContext<CatalogoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
