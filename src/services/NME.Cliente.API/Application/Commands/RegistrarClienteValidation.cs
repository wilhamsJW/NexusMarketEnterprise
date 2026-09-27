using System;
using FluentValidation;
using NME.Core.DomainObjects;

namespace NME.Clientes.API.Application.Commands
{
    /// <summary>
    /// OBJETIVO: Define o conjunto de regras de validação para o comando RegistrarClienteCommand.
    /// Padrão: FluentValidation / Design by Contract.
    /// </summary>
    public class RegistrarClienteValidation : AbstractValidator<RegistrarClienteCommand>
    {
        public RegistrarClienteValidation()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do cliente inválido");

            RuleFor(c => c.Nome)
                .NotEmpty()
                .WithMessage("O nome do cliente não foi informado");

            RuleFor(c => c.Cpf)
                .Must(TerCpfValido)
                .WithMessage("O CPF informado não é válido.");

            RuleFor(c => c.Email)
                .Must(TerEmailValido)
                .WithMessage("O e-mail informado não é válido.");
        }

        // Utiliza a validação estática encapsulada no Value Object Cpf
        protected static bool TerCpfValido(string cpf)
        {
            return Cpf.Validar(cpf);
        }

        // Utiliza a validação estática encapsulada no Value Object Email
        protected static bool TerEmailValido(string email)
        {
            return Email.Validar(email);
        }
    }
}

//Qual é o objetivo desta classe?
//A RegistrarClienteValidation centraliza as validações de entrada da requisição antes mesmo de tentarmos 
//    tocar no banco de dados ou instanciar uma entidade do domínio.

//Ela herda de AbstractValidator<RegistrarClienteCommand> da biblioteca FluentValidation e utiliza os 
//    nossos métodos estáticos dos Value Objects (Cpf.Validar e Email.Validar) para checar se o formato 
//    dos dados está correto.

//Por baixo dos panos (Mecanismos C# e POO)
//Must(TerCpfValido): O método .Must() do FluentValidation aceita um ponteiro para uma função 
//    booleana (um predicate). Ele passa a propriedade c.Cpf como argumento para TerCpfValido e, 
//    se o retorno for false, aciona a mensagem de erro configurada em .WithMessage().

//Reaproveitamento de Regra (DRY): Não escrevemos a lógica de validação de CPF/E-mail de novo aqui. 
//    Apenas delegamos para Cpf.Validar() e Email.Validar(), mantendo a fonte única da verdade da 
//    regra de negócio.