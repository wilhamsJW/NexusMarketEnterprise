using FluentValidation.Results;
using MediatR;

namespace NME.Core.Messages
{
    /// <summary>
    /// OBJETIVO: Serve como classe abstrata base para todos os Comandos (ações de escrita/alteração) da aplicação.
    /// Padrão: CQRS / Command Pattern.
    /// </summary>
    public abstract class Command : Message, IRequest<ValidationResult>
    {
        // Guarda a data e hora exatas em que o comando foi criado na memória
        public DateTime Timestamp { get; private set; }

        // Guarda o resultado das validações executadas pelo FluentValidation
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            // Define o Timestamp automaticamente no momento em que o comando é instanciado
            Timestamp = DateTime.Now;
        }

        // Método virtual que será sobrescrito em cada Command específico para rodar a validação do FluentValidation
        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }
    }
}

// Por baixo dos panos (Mecanismos C# e POO)
// IRequest<ValidationResult>: É uma interface de marcação do MediatR. Ao assinar isso, você está dizendo ao C# e ao
// MediatR: 
// "Esta classe é um comando oficial que vai trafegar no barramento, e quando o processamento acabar, ele obrigatoriamente 
// deve retornar um ValidationResult".

//    virtual bool EhValido(): A palavra-chave virtual significa que a classe filha 
//    (ex: RegistrarClienteCommand) pode sobrescrever (override) este método com a sua 
//    própria lógica de validação. Se a classe filha não sobrescrever e tentar chamar o método, 
//    ele lança uma exceção NotImplementedException.