using MediatR;
using Microsoft.EntityFrameworkCore;
using NME.Cliente.API.Configuration;
using NME.Cliente.API.Data;

namespace NME.Clientes.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Configuração do Entity Framework Core / DbContext
            builder.Services.AddDbContext<ClientesContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configuração do MediatR
            // Varre todo o assembly procurando automaticamente por classes que implementam 
            // as interfaces do MediatR (como IRequestHandler para Commands/Queries, 
            // INotificationHandler para Events, e IPipelineBehavior para validações/logs).
            // Ao encontrá-las, cadastra-as no container de Injeção de Dependências, 
            // eliminando a necessidade de registro manual individual.
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            // Injeção de Dependência dos nossos Repositórios e Handlers
            builder.Services.RegisterServices();

            // Configuração do Swagger/OpenAPI
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