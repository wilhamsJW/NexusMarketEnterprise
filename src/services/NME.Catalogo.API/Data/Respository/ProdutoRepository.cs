using Microsoft.EntityFrameworkCore;
using NME.Catalogo.API.Models;
using NME.Core.DomainObjects.Data;

namespace NME.Catalogo.API.Data.Respository
{
    /// <summary>
    /// Implementação concreta do Repositório de Produtos usando o Entity Framework Core.
    /// Realiza a ponte entre a interface do Domínio (IProdutoRepository) e o Banco de Dados.
    /// </summary>
    public class ProdutoRepository : IProdutoRepository
    {
        // Instância do DbContext que gerencia a sessão com o SQL Server
        private readonly CatalogoContext _context;

        /// <summary>
        /// Construtor que recebe a instância do DbContext via Injeção de Dependência.
        /// </summary>
        public ProdutoRepository(CatalogoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Expõe o DbContext como UnitOfWork (Unidade de Trabalho).
        /// Permite que a camada superior (Handler/Controller) faça o Commit (SaveChanges)
        /// de todas as alterações em uma única transação de banco.
        /// </summary>
        public IUnitOfWork UnitOfWork => _context;

        /// <summary>
        /// Busca todos os produtos gravados no banco de dados.
        /// </summary>
        /// <returns>Uma coleção assíncrona de produtos.</returns>
        public async Task<IEnumerable<Produto>> ObterTodos()
        {
            // AsNoTracking(): Desativa o rastreamento do EF Core na memória RAM.
            // Isso ganha bastante performance e reduz consumo de memória em consultas somente de leitura.
            return await _context.Produtos.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Busca um produto específico pelo seu identificador único (Guid).
        /// </summary>
        public async Task<Produto?> ObterPorId(Guid id)
        {
            // FindAsync: Procura no banco pela Chave Primária (PK).
            // Primeiro busca se o objeto já está carregado na memória do EF Core; se não estiver, vai ao banco.
            return await _context.Produtos.FindAsync(id);
        }

        /// <summary>
        /// Adiciona um novo produto ao contexto do EF Core.
        /// </summary>
        /// <remarks>
        /// Não executa o INSERT no banco imediatamente (retorna void).
        /// Apenas marca o estado do objeto como 'Added' na memória.
        /// </remarks>
        public void Adicionar(Produto produto)
        {
            _context.Produtos.Add(produto);
        }

        /// <summary>
        /// Atualiza as propriedades de um produto existente.
        /// </summary>
        /// <remarks>
        /// Não executa o UPDATE no banco imediatamente (retorna void).
        /// Apenas marca o estado do objeto como 'Modified' na memória.
        /// </remarks>
        public void Atualizar(Produto produto)
        {
            _context.Produtos.Update(produto);
        }

        /// <summary>
        /// Libera a conexão com o banco de dados e limpa a memória alocada pelo DbContext.
        /// Chamado automaticamente pelo .NET ao finalizar o ciclo de vida da requisição HTTP.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}