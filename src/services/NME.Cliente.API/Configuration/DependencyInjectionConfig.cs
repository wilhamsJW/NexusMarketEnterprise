using FluentValidation.Results;
using MediatR;
using NME.Cliente.API.Data;
using NME.Cliente.API.Data.Repository;
using NME.Cliente.API.Domain.Interface;
using NME.Clientes.API.Application.Commands;

namespace NME.Cliente.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Application / Commands
            services.AddScoped<IRequestHandler<RegistrarClienteCommand, ValidationResult>, ClienteCommandHandler>();

            // Data / Repository / Context
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<ClientesContext>();
        }
    }
}