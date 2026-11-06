using MediatR;
using Microsoft.EntityFrameworkCore;
using NME.Core.DomainObjects;

namespace NME.Core.Mediator
{
    /// <summary>
    /// CLASSE DE SUPORTE (Extension Method): Responsável por interceptar o EF Core, 
    /// extrair os eventos de domínio de dentro das entidades e despachá-los para o MediatR.
    /// Padrão Arquitetural: Interceptor de Eventos / Mediator Pattern.
    /// </summary>
    public static class MediatorExtension
    {
        /// <summary>
        /// Este método estende a interface IMediator. 
        /// O 'this IMediator mediator' significa que qualquer objeto do tipo IMediator 
        /// no seu projeto agora ganha o superpoder de chamar '.PublicarEventos(context)'.
        /// </summary>
        public static async Task PublicarEventos<T>(this IMediator mediator, T ctx) where T : DbContext
        {
            // 1. O EF Core rastreia tudo o que mudou na memória. 
            // Aqui pegamos todas as entidades que herdam de 'Entity' e que possuem notificações na memoria.
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            // 2. Extraímos todas as "cartas" (eventos) de todas as entidades encontradas em uma lista única.
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            // 3. Limpamos imediatamente a memoria das entidades para evitar envio duplicado de eventos.
            domainEntities.ToList()
                .ForEach(entity => entity.Entity.LimparEventos());

            // 4. Passamos cada evento extraído para o MediatR publicar para os Handlers correspondentes.
            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
