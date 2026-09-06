using Microsoft.EntityFrameworkCore;
using NME.Catalogo.API.Models;
using NME.Core.DomainObjects.Data;

namespace NME.Catalogo.API.Data
{
    // CatalogoContext herda o comportamento de banco do 'DbContext' 
    // e implementa o contrato de transação do DDD 'IUnitOfWork'
    public class CatalogoContext : DbContext, IUnitOfWork
    {
        // CONSTRUTOR:
        // Recebe as configurações (como ConnectionString) via Injeção de Dependência
        // e repassa para a classe pai (base = DbContext) fazer a inicialização do motor do EF Core.
        public CatalogoContext(DbContextOptions<CatalogoContext> options)
            : base(options) { }

        // Mapeia a entidade 'Produto' para ser manipulada no C#.
        // Funciona como a representação em memória da tabela do banco.
        public DbSet<Produto> Produtos { get; set; }

        // Método executado no momento da construção do modelo de dados no banco.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TRAVA DE SEGURANÇA (MAPPING DEFAULT):
            // Varre todas as propriedades do tipo 'string' de todas as entidades do sistema.
            // Se alguma string não teve seu tamanho mapeado explicitamente no ProdutoMapping,
            // ela será configurada automaticamente como 'varchar(100)' em vez do pesado 'nvarchar(max)'.
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            // REGISTRO AUTOMÁTICO DOS MAPPINGS:
            // Procura dentro deste assembly (projeto) todas as classes que implementam 
            // 'IEntityTypeConfiguration' (ex: ProdutoMapping) e aplica as regras automaticamente.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogoContext).Assembly);
        }

        // IMPLEMENTAÇÃO DO PADRÃO UNIT OF WORK:
        // Centraliza a gravação das alterações do banco em um único ponto transacional.
        public async Task<bool> Commit()
        {
            // SaveChangesAsync() envia todas as alterações (Add, Update, Delete) para o SQL Server.
            // Se o número de linhas afetadas for maior que 0, retorna 'true' (sucesso).
            return await base.SaveChangesAsync() > 0;
        }
    }
}