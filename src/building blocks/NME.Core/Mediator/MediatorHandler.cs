using FluentValidation.Results;
using MediatR;
using NME.Core.Messages;

namespace NME.Core.Mediator
{
    /// <summary>
    /// OBJETIVO: Implementa o despacho de mensagens usando o pacote MediatR por baixo dos panos.
    /// Padrão: Mediator / Adapter Pattern.
    /// </summary>
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;

        // Injeção de dependência do IMediator nativo da biblioteca MediatR
        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Recebe um Comando genérico do nosso sistema e repassa para o .Send() do MediatR
        public async Task<ValidationResult> EnviarComando<T>(T comando) where T : Command
        {
            return await _mediator.Send(comando);
        }

        // Recebe um Evento genérico do nosso sistema e repassa para o .Publish() do MediatR
        public async Task PublicarEvento<T>(T evento) where T : Event
        {
            await _mediator.Publish(evento);
        }
    }
}

//Qual é o objetivo desta classe?
//A MediatorHandler é a implementação concreta da interface IMediatorHandler.

//O objetivo dela é atuar como o despachante/orquestrador interno da aplicação. Ela recebe a chamada de abstração
//    do nosso sistema, injeta o IMediator do pacote MediatR via construtor, e repassa a execução para os
//    métodos nativos .Send() (para Comandos) e .Publish() (para Eventos).

//Por baixo dos panos (Mecanismos C# e POO)
//private readonly IMediator _mediator: O readonly garante que a instância do MediatR só possa ser atribuída no
//    construtor, evitando reatribuições acidentais durante a execução da classe.

//await _mediator.Send(comando): O método .Send() do MediatR busca automaticamente no contêiner de 
//    Injeção de Dependências qual IRequestHandler está registrado para aquele comando específico e dispara a 
//    execução assíncrona.

//await _mediator.Publish(evento): O método .Publish() dispara a notificação para todos os INotificationHandler
//    registrados para aquele evento sem esperar um retorno direto de dados.
