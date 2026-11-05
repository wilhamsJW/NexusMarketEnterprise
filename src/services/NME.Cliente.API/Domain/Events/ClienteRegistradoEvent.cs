using NME.Core.Messages;

namespace NME.Cliente.API.Domain.Events
{
    /// <summary>
    /// OBJETIVO: Representa o fato histórico de que um novo cliente foi registrado com sucesso no sistema.
    /// Padrão: Domain Events.
    /// </summary>
    public class ClienteRegistradoEvent : Event
    {
        // Identificador único do cliente que foi cadastrado
        public Guid Id { get; private set; }

        // Nome do cliente cadastrado
        public string Nome { get; private set; }

        // E-mail do cliente (usado posteriormente pelo ouvinte de e-mail)
        public string Email { get; private set; }

        public ClienteRegistradoEvent(Guid id, string nome, string email)
        {
            Id = id;
            AggregateId = id; // Associa ao AggregateId herdado da classe Message para rastreabilidade
            Nome = nome;
            Email = email;
        }
    }
}

//    Conceito de POO e Arquitetura
//    Transporte de Dados do Fato: O ClienteRegistradoEvent funciona como um "telegrama"
//    contendo exatamente o que os ouvintes precisam saber (o ID, o Nome e o E-mail).
//    Ele não tem lógica de negócio; ele é apenas uma estrutura imutável de dados que guarda o
//    que aconteceu.
