using Microsoft.EntityFrameworkCore;
using NME.Cliente.API.Clientes;
using NME.Cliente.API.Domain.Interface;
using NME.Core.DomainObjects.Data;

namespace NME.Cliente.API.Data.Repository
{
    /// <summary>
    /// OBJETIVO: Acesso a dados e persistência da entidade Cliente via EF Core.
    /// Padrão: Repository Pattern.
    /// </summary>
    public class ClienteRepository : IClienteRepository
    {
        private readonly ClientesContext _context;

        public ClienteRepository(ClientesContext context)
        {
            _context = context;
        }

        // Propriedade herdada para expor a Unidade de Trabalho
        public IUnitOfWork UnitOfWork => _context;

        public void Adicionar(Client cliente)
        {
            _context.Clientes.Add(cliente);
        }

        public async Task<IEnumerable<Client>> ObterTodos()
        {
            return await _context.Clientes.AsNoTracking().ToListAsync();
        }

        public async Task<Client?> ObterPorCpf(string cpf)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Numero == cpf);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

//Por baixo dos panos (Mecanismos C# e POO)
//IUnitOfWork UnitOfWork => _context;: A expressão de membro com seta (=>) devolve a
//    instância do próprio DbContext. Como o ClientesContext implementa IUnitOfWork, 
//    ele é retornado diretamente.

//AsNoTracking(): Utilizado nas consultas de leitura (ObterTodos). Diz ao EF Core
//    para não rastrear as entidades em memória, o que melhora a performance e reduz
//    o consumo de memória.

//IDisposable: A interface exige o método Dispose(), que desaloca a conexão com o
//    banco de dados quando a requisição é finalizada.
