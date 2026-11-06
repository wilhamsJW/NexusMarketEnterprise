using MediatR;

namespace NME.Cliente.API.Domain.Events.Handlers
{
    /// <summary>
    /// OBJETIVO: Escuta o evento 'ClienteRegistradoEvent' e executa a ação secundária 
    /// (side effect) de simular o envio de um e-mail de boas-vindas.
    /// Padrão Arquitetural: Domain Event Handler / Observer Pattern (via MediatR).
    /// </summary>
    public class ClienteEventHandler : INotificationHandler<ClienteRegistradoEvent>
    {
        /// <summary>
        /// Este método é chamado automaticamente pelo MediatR assim que o evento é publicado.
        /// Recebe a "carta" (notification) com os dados do cliente recém-cadastrado.
        /// </summary>
        public Task Handle(ClienteRegistradoEvent notification, CancellationToken cancellationToken)
        {
            // Simulando o envio do e-mail de boas-vindas no console 
            // (que posteriormente você poderá testar integrando com o Mailpit).
            System.Console.WriteLine("--------------------------------------------------");
            System.Console.WriteLine($"[EMAIL DE BOAS-VINDAS DISPARADO COM SUCESSO]");
            System.Console.WriteLine($"Para: {notification.Email}");
            System.Console.WriteLine($"Cliente: {notification.Nome} (CPF: {notification.Cpf})");
            System.Console.WriteLine($"Data/Hora do Evento: {notification.Timestamp}");
            System.Console.WriteLine("--------------------------------------------------\n");

            // Como aqui não estamos fazendo nenhuma chamada assíncrona pesada (como await de banco),
            // retornamos uma Task concluída com sucesso.
            return Task.CompletedTask;
        }
    }
}