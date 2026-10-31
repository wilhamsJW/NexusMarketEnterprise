using FluentValidation.Results;
using MediatR;
using NME.Cliente.API.Data;
using NME.Cliente.API.Data.Repository;
using NME.Cliente.API.Domain.Interface;
using NME.Clientes.API.Application.Commands;
using NME.Clientes.API.Application.Handlers;
using NME.Core.Mediator;

namespace NME.Cliente.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // =========================================================================
            // REGISTRO DO MEDIATR (Interface Genérica de Comando + Handler)
            // =========================================================================
            // Explicação da sintaxe: O IRequestHandler<> é uma interface genérica do MediatR.
            // - RegistrarClienteCommand = O que ENTRA (o comando que vai ser processado).
            // - ValidationResult = O que SAI (a resposta com o resultado das validações).
            // - ClienteCommandHandler = A classe concreta (a implementação de fato que roda a regra).
            services.AddScoped<IRequestHandler<RegistrarClienteCommand, ValidationResult>, ClienteCommandHandler>();

            // =========================================================================
            // REGISTRO DE REPOSITÓRIOS (Interface Direta + Implementação)
            // =========================================================================
            // Explicação da sintaxe: Mapeamento simples de contrato para implementação.
            // - IClienteRepository = A interface (contrato).
            // - ClienteRepository = A classe concreta (implementação que acessa o banco).
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<ClientesContext>();

            services.AddScoped<IMediatorHandler, MediatorHandler>();
        }
    }
}