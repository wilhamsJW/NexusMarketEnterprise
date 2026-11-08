using System.Reflection.Metadata;
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
            // O domainEvents guarda a instância real do objeto criado (new ClienteRegistradoEvent(...)
            // O C# carimbou essa instância com a sua assinatura de tipo real (ClienteRegistradoEvent).
            // 
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            // 3. Limpamos imediatamente a memoria das entidades para evitar envio duplicado de eventos.
            domainEntities.ToList()
                .ForEach(entity => entity.Entity.LimparEventos());

            // 4. Passamos cada evento extraído para o MediatR publicar para os Handlers correspondentes.
            // Caso tenha mais de um evento ele vai ser disparado aqui, 
            // Exemplo de disparo de um evento na linha: cliente.AdicionarEvento(new ClienteRegistradoEvent(message.Id, message.Nome, message.EmailClient));
            // Na class ClienteCommandHandler
            foreach (var domainEvent in domainEvents)
            {
                //MediatR chama o método Handle do Handler correto sem precisar saber qual é a classe exata do
                //evento em tempo de desenvolvimento. Ele descobre e executa o comportamento específico daquele 
                //evento em runtime.
                await mediator.Publish(domainEvent);
            }
        }
    }
}

//*** Fluxo de como o Mediatr descobre o handler para ser executado, vale
// lembrar que o Mediatr olha pra o nome da instancia guardado, mesmo que passemos apenas o objeto na linha
// await mediator.Publish(domainEvent); o mediator descobre qual é a instacia e envia para o handler que esta
// com a marcacao correta, exemplo: public class ClienteEventHandler : INotificationHandler<ClienteRegistradoEvent>
// e o ClienteEventHandler recebe os dados do evento para fazer o que quiser atraves do parametro notification


// 1 - O domainEvents guarda a instância real do objeto criado (new ClienteRegistradoEvent(...)).   

// 2 - O C# carimbou essa instância com a sua assinatura de tipo real (ClienteRegistradoEvent).   

// 3 - O mediator.Publish(domainEvent) pega essa instância, olha por baixo dos panos o tipo real dela 
// (sem precisar olhar para propriedades como MessageType).   

// 4 - Ele cruza esse tipo com o dicionário que ele montou na inicialização da aplicação, descobre que existe 
// um ClienteEventHandler assinando a interface INotificationHandler<ClienteRegistradoEvent>.   

//    5 - Por fim, ele injeta os dados exatamente no parâmetro notification (public Task Handle(ClienteRegistradoEvent 
//    notification, CancellationToken cancellationToken)), permitindo que o Handler faça o que quiser com aquelas 
//    informações (como disparar o e-mail no console)!   
