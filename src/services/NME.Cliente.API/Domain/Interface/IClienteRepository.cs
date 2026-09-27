using NME.Core.DomainObjects.Data;
using NME.Cliente.API.Clientes;

namespace NME.Cliente.API.Domain.Interface
{
    /// <summary>
    /// OBJETIVO: Define o contrato de persistência para a entidade Cliente no banco de dados.
    /// Padrão: DDD (Repository Pattern / Aggregate Root).
    /// </summary>
    public interface IClienteRepository : IRepository<Client>
    {
        // Adiciona um novo cliente no contexto do banco
        void Adicionar(Client cliente);

        // Busca a lista completa de clientes cadastrados
        Task<IEnumerable<Client>> ObterTodos();

        // Busca um cliente específico através do seu CPF
        Task<Client?> ObterPorCpf(string cpf);
    }
}

//Qual é o objetivo desta interface?
//A IClienteRepository é o contrato de persistência da entidade Cliente.

//No DDD (Domain-Driven Design), o Domínio define o que precisa ser feito no banco de dados 
//(Adicionar, Obter por CPF, Obter Todos), mas não se preocupa em como isso é feito 
//(se é com Entity Framework, Dapper ou SQL puro). Ela herda de IRepository<Cliente> 
//para garantir o vínculo com a raiz de agregação (Aggregate Root) e com a unidade de trabalho
//(IUnitOfWork).

//Por baixo dos panos (Mecanismos C# e POO)
//IRepository<Cliente>: Esta interface genérica do NME.Core.Data expõe a propriedade IUnitOfWork 
//    UnitOfWork { get; } e impõe que qualquer repositório seja associado a uma raiz de agregação
//    (Aggregate Root).

//Task<IEnumerable<Cliente>>: Indica chamadas assíncronas de leitura que retornarão uma coleção
//    iterável de clientes do banco sem travar a thread de execução da API.