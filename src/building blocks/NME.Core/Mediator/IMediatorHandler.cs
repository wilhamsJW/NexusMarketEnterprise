using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NME.Core.Messages;

namespace NME.Core.Mediator
{
    /// <summary>
    /// OBJETIVO: Contrato que define as ações do barramento Mediator na aplicação.
    /// Padrão: Mediator / Facade / Dependency Inversion Principle (D do SOLID).
    /// </summary>
    public interface IMediatorHandler
    {
        // Envia um Comando de alteração para o barramento e aguarda o resultado da validação
        Task<ValidationResult> EnviarComando<T>(T comando) where T : Command;

        // Publica um Evento para todos os ouvintes/handlers interessados na aplicação
        Task PublicarEvento<T>(T evento) where T : Event;
    }
}

//Qual é o objetivo desta interface?
//A IMediatorHandler é a abstração (contrato) do nosso barramento/mediador.

//Em vez de injetar a interface nativa do pacote MediatR(IMediator) diretamente nas nossas Controllers ou
//    Consumidores, nós criamos este contrato próprio.

//Isso desacopla nossa aplicação da biblioteca de terceiros e estabelece exatamente duas ações que 
//    nosso barramento pode fazer: EnviarComando(escrita) e PublicarEvento(notificação).

//Por baixo dos panos (Mecanismos C# e POO)
//Generics (<T>) e Restrições (where T : Command): O<T> permite que o método receba qualquer tipo de comando
//    (ex: RegistrarClienteCommand, InativarClienteCommand). A cláusula where T : Command garante que apenas 
//    classes que herdem da nossa classe base Command possam ser passadas para este método.

//Task<ValidationResult>: Indica um método assíncrono que retornará no futuro o resultado da validação após 
//    o Handler processar o comando.