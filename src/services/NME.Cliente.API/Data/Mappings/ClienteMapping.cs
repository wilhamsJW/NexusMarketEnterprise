using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NME.Cliente.API.Clientes;
using NME.Core.DomainObjects;

namespace NME.Cliente.API.Data.Mappings
{
    /// <summary>
    /// OBJETIVO: Mapeamento ORM (Fluent API) da entidade de domínio Cliente.
    /// Padrão: Entity Type Configuration / Data Mapper.
    /// </summary>
    public class ClienteMapping : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            // Define a chave primária
            builder.HasKey(c => c.Id);

            // Mapeia propriedades simples
            builder.Property(c => c.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(c => c.Excluido)
                .IsRequired();

            // --- Mapeamento de Value Objects (Owned Entities) ---

            // Mapeia o Value Object Email
            builder.OwnsOne(c => c.Email, tf =>
            {
                tf.Property(c => c.Endereco)
                    .IsRequired()
                    .HasColumnName("Email")
                    .HasColumnType($"varchar({Email.EmailMaxEndereco})");
            });

            // Mapeia o Value Object Cpf
            builder.OwnsOne(c => c.Cpf, tf =>
            {
                tf.Property(c => c.Numero)
                    .IsRequired()
                    .HasMaxLength(Cpf.CpfMaxNumero)
                    .HasColumnName("Cpf")
                    .HasColumnType($"varchar({Cpf.CpfMaxNumero})");
            });

            // Mapeia o Value Object Endereco
            builder.OwnsOne(c => c.Endereco, tf =>
            {
                tf.Property(e => e.Logradouro)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                tf.Property(e => e.Numero)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                tf.Property(e => e.Cep)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                tf.Property(e => e.Bairro)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                tf.Property(e => e.Cidade)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                tf.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                tf.Property(e => e.Complemento)
                    .HasColumnType("varchar(250)");
            });

            // Nome da tabela no banco de dados
            builder.ToTable("Clientes");
        }
    }
}

//Por baixo dos panos (Conceito POO/C# simples)
//Constantes Estáticas (public const): Ao definir public const int 
//    EmailMaxEndereco = 254; dentro da classe Email, o C# permite acessar esse valor
//    diretamente usando Email.EmailMaxEndereco, sem precisar instanciar um objeto 
//    com new Email().

//DRY (Don't Repeat Yourself): Reutilizar a constante da regra de domínio 
//    dentro do mapeamento do banco garante que o limite de caracteres da validação
//    em memória seja idêntico ao tamanho da coluna varchar no SQL Server.