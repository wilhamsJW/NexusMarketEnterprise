using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NME.Identidade.API.Data
{
    // P: IdentityDbContext - de onde vem e o que representa?
    // R: Vem do pacote "Microsoft.AspNetCore.Identity.EntityFrameworkCore".
    //    Representa a classe base da Microsoft que já possui, prontos por baixo dos panos,
    //    todos os mapeamentos e conjuntos de tabelas (DbSet) para gerenciamento de usuários,
    //    senhas, papéis (roles) e permissões (claims), como AspNetUsers e AspNetRoles.
    public class ApplicationDbContext : IdentityDbContext
    {
        // P: DbContextOptions - de onde vem e o que representa?
        // R: Vem do pacote "Microsoft.EntityFrameworkCore".
        //    Representa a caixa/objeto que carrega as configurações de conexão do banco de dados 
        //    (como o tipo do banco SQL Server, a ConnectionString, senhas, etc.).
        //
        // P: Por que os nomes se repetem em DbContextOptions<ApplicationDbContext> e no nome do construtor ApplicationDbContext?
        // R: O nome "ApplicationDbContext" no construtor é o nome do método de criação da sua classe.
        //    O nome dentro dos sinais de menor e maior <ApplicationDbContext> é um Generic do C# que funciona 
        //    como uma "etiqueta destinatária". Ele garante ao .NET que esta caixa de opções pertence EXCLUSIVAMENTE 
        //    a este banco de dados (ApplicationDbContext) e não a outro banco da aplicação.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)

            // P: Por que devo usar : base(options) aqui?
            // R: O "base" chama o construtor da classe Pai (IdentityDbContext). 
            //    Serve para pegar a variável "options" (que contém a ConnectionString e as configurações recebidas) 
            //    e repassá-la para o motor interno da Microsoft conseguir conectar ao banco e criar a estrutura do Identity.
            : base(options)
        {
            // O corpo do construtor fica vazio porque a inicialização é feita pela classe pai via base(options).
        }
    }
}
