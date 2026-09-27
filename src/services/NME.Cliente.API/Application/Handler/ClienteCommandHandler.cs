using FluentValidation.Results;
using MediatR;
using NME.Cliente.API.Clientes;
using NME.Cliente.API.Domain.Interface;
using NME.Clientes.API.Application.Commands;
using NME.Core.DomainObjects;
using NME.Core.Messages;

namespace NME.Clientes.API.Application.Handlers
{
    /// <summary>
    /// OBJETIVO: Processa a intenção de registro do cliente aplicando validações e persistência.
    /// Padrão: CQRS / Command Handler Pattern.
    /// </summary>
    public class ClienteCommandHandler : CommandHandler,
        IRequestHandler<RegistrarClienteCommand, ValidationResult>
    {
        private readonly IClienteRepository _clienteRepository;

        // Injeção de dependência do repositório da entidade Cliente
        public ClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ValidationResult> Handle(RegistrarClienteCommand message, CancellationToken cancellationToken)
        {
            // 1. Validação da própria mensagem (Sintaxe e FluentValidation)
            if (!message.EhValido()) return message.ValidationResult;

            // 2. Criação das instâncias de Value Objects e Entidade de Domínio
            var cliente = new Client(message.Id, message.Nome, new Email(message.EmailClient), new Cpf(message.CpfClient));

            // 3. Validação de regra de negócio (Verifica se CPF já está cadastrado)
            var clienteExistente = await _clienteRepository.ObterPorCpf(cliente.Cpf.Numero);

            if (clienteExistente != null)
            {
                AdicionarErro("Este CPF já está em uso.");
                return ValidationResult;
            }

            // 4. Adiciona o cliente na memória do DbContext
            _clienteRepository.Adicionar(cliente);

            // 5. Persiste as alterações no banco usando o método Herdado de CommandHandler
            return await PersistirDados(_clienteRepository.UnitOfWork);
        }
    }
}

//Qual é o objetivo desta classe?
//O ClienteCommandHandler é o orquestrador do caso de uso de registrar cliente.

//Ele herda da nossa classe base CommandHandler (criada na Etapa 1) e implementa a interface
//    IRequestHandler<RegistrarClienteCommand, ValidationResult> do MediatR.

//Sua responsabilidade é:

//Validar a sintaxe da mensagem chamando message.EhValido().

//Instanciar a entidade Cliente com os Value Objects (Email e Cpf).

//Verificar a regra de negócio de duplicidade no banco via _clienteRepository.ObterPorCpf().

//Persistir a entidade chamando PersistirDados(_clienteRepository.UnitOfWork).

//Por baixo dos panos (Mecanismos C# e POO)
//IRequestHandler<RegistrarClienteCommand, ValidationResult>: Contrato nativo do MediatR que diz: 
//    "Quando um RegistrarClienteCommand for enviado no barramento, esta classe é quem vai responder retornando um ValidationResult".

//AdicionarErro("Este CPF já está em uso."): Utiliza o método protegido herdado da nossa classe base
//    CommandHandler. Ele injeta a falha dentro do objeto ValidationResult que será retornado até a 
//    Controller.

//PersistirDados(_clienteRepository.UnitOfWork): O repositório expõe a unidade de trabalho (UnitOfWork).
//    O método herdado faz o Commit() e garante que se o banco falhar, o erro será tratado uniformemente.