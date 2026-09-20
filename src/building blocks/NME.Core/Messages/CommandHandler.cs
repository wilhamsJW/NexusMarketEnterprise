using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation.Results;
using NME.Core.Data;
using NME.Core.DomainObjects.Data;

namespace NME.Core.Messages
{
    /// <summary>
    /// OBJETIVO: Fornece utilitários de validação e persistência para serem herdados pelos Handlers de negócio.
    /// Padrão: CQRS / Dry (Don't Repeat Yourself) / Unit of Work Pattern.
    /// </summary>
    public abstract class CommandHandler
    {
        // Propriedade protegida que acumula as falhas de validação durante o fluxo do Handler
        protected ValidationResult ValidationResult;

        protected CommandHandler()
        {
            // Inicializa a coleção de validações vazia ao criar o Handler
            ValidationResult = new ValidationResult();
        }

        // Adiciona um erro personalizado na lista de falhas que retornará para a Controller
        protected void AdicionarErro(string mensagem)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
        }

        // Chama o Commit do banco via IUnitOfWork e intercepta se houve falha na persistência
        protected async Task<ValidationResult> PersistirDados(IUnitOfWork uow)
        {
            if (!await uow.Commit()) AdicionarErro("Houve um erro ao persistir os dados");

            return ValidationResult;
        }
    }
}

//Qual é o objetivo desta classe?
//A CommandHandler é uma classe abstrata utilitária que serve de mãe para todos os manipuladores de comandos do
//    sistema (ex: ClienteCommandHandler).

//O objetivo dela é fornecer uma estrutura centralizada para gerenciar o objeto de erros (ValidationResult), 
//adicionar novas falhas durante o processamento e lidar com a chamada de persistência no banco de dados através
//da unidade de trabalho (IUnitOfWork).

//Por baixo dos panos (Mecanismos C# e POO)
//protected ValidationResult: O modificador protected permite que todas as classes filhas(os Handlers específicos) 
//    acessem e manipulem diretamente o ValidationResult para incluir mensagens de erro do negócio 
//    (ex: "CPF já cadastrado").

//IUnitOfWork uow: Interface do padrão Unit of Work (unidade de trabalho). Em vez do Handler chamar _context.SaveChanges(), ele chama uow.Commit(). Se o banco falhar ao salvar, a classe base adiciona a mensagem de erro na lista e retorna tudo de forma 
//    padronizada.