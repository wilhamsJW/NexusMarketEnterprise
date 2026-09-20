using System;
using MediatR;

namespace NME.Core.Messages
{
    /// <summary>
    /// OBJETIVO: Serve como classe abstrata base para todos os Eventos de Domínio do sistema.
    /// Padrão: Event-Driven Architecture / Domain Events.
    /// </summary>
    public abstract class Event : Message, INotification
    {
        // Data e hora exatas em que o evento ocorreu
        public DateTime Timestamp { get; private set; }

        protected Event()
        {
            // Registra o momento em que o fato/evento ocorreu na memória
            Timestamp = DateTime.Now;
        }
    }
}

//Qual é o objetivo desta classe?
//A Event é a classe base para qualquer fato que já aconteceu no sistema (ex: ClienteRegistradoEvent).

//Diferente do Command (que é uma intenção que pode ser rejeitada), o Event representa um fato passado que 
//já foi processado e salvo. Ela herda de Message e implementa INotification do MediatR, permitindo que vários 
//ouvintes (subscribers) reajam a esse fato ao mesmo tempo.

//Por baixo dos panos (Mecanismos C# e POO)
//INotification: É a interface do MediatR usada para o padrão Publish/Subscribe. Enquanto o IRequest (do Command) 
//    envia uma mensagem para apenas um Handler, o INotification (do Event) pode ser notificado para zero ou múltiplos
//    Handlers ao mesmo tempo (ex: ao registrar um cliente, dispara um evento para enviar e-mail de boas-vindas
//    E outro evento para gerar log).