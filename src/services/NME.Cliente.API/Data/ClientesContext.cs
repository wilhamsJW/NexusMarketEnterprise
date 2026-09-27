using Microsoft.EntityFrameworkCore;
using NME.Cliente.API.Clientes;
using NME.Core.DomainObjects.Data;

namespace NME.Cliente.API.Data
{
    /// <summary>
    /// OBJETIVO: Gerencia as sessões de conexão com o banco de dados e executa o padrão UnitOfWork.
    /// Padrão: Entity Framework Core / Data Mapper.
    /// </summary>
    public class ClientesContext : DbContext, IUnitOfWork
    {
        public ClientesContext(DbContextOptions<ClientesContext> options) : base(options) { }

        public DbSet<Client> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica automaticamente todos os mapeamentos (IEntityTypeConfiguration) deste Assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientesContext).Assembly);

            // Desativa o DeleteCascade globalmente para evitar deleção acidental de registros filhos
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }

            base.OnModelCreating(modelBuilder);
        }

        // Implementação da interface IUnitOfWork
        public async Task<bool> Commit()
        {
            // Retorna true se pelo menos uma linha foi afetada no banco de dados
            return await base.SaveChangesAsync() > 0;
        }
    }
}

//Por baixo dos panos (Mecanismos C# e POO)
//ApplyConfigurationsFromAssembly: Em vez de registrar classe por classe de mapeamento
//    (modelBuilder.ApplyConfiguration(...)), este método varre o projeto em tempo 
//    de execução e aplica todas as regras de Fluent API automaticamente.

//IUnitOfWork e Commit(): O DbContext do EF Core já é uma implementação nativa do
//    padrão Unit of Work. O método SaveChangesAsync() envia as alterações para o 
//    banco em uma única transação atômica.

//DeleteBehavior.ClientSetNull: Impede que o EF Core crie regras de exclusão em
//    cascata (Cascading Delete) no banco de dados relacional, forçando o desenvolvedor
//    a tratar explicitamente a remoção de agregados.