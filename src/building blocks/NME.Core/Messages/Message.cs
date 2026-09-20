using System;

namespace NME.Core.Messages
{
    /// <summary>
    /// OBJETIVO: Serve como classe abstrata base para qualquer tipo de mensagem no sistema (Commands, Events, etc).
    /// Padrão: CQRS / Event-Driven Architecture.
    /// </summary>
    public abstract class Message
    {
        // Define o tipo da mensagem (recebe o nome da classe filha que a herdar, ex: "RegistrarClienteCommand")
        public string MessageType { get; protected set; }

        // Identificador único do Agregado/Entidade que esta mensagem pretende manipular (ex: Id do Cliente)
        public Guid AggregateId { get; protected set; }

        protected Message()
        {
            // Ao instanciar a mensagem, preenchemos o tipo automaticamente com o nome da classe real
            MessageType = GetType().Name;
        }
    }
}