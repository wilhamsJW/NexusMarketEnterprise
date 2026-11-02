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
            // Define a chave primária da tabela
            builder.HasKey(c => c.Id);

            // Mapeia propriedades primitivas simples
            builder.Property(c => c.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(c => c.Excluido)
                .IsRequired();

            // =========================================================================
            // --- MAPEAMENTO DE VALUE OBJECTS (Conceito DDD: Owned Entities) ---
            // O que é: O EF Core vai "desmontar" os objetos de valor (Email, Cpf, Endereco)
            // e gravar as propriedades deles diretamente nas colunas da tabela "Clientes",
            // mantendo a classe de domínio rica e encapsulada em C#.
            // =========================================================================

            // Mapeia o Value Object Email
            builder.OwnsOne(c => c.Email, tf =>
            {
                // REVISÃO (DRY - Sincronia Domínio x Banco):
                // Usamos a constante estática (Email.EmailMaxEndereco) criada lá na classe 
                // de Domínio do Email. Isso impede "magic numbers" (números mágicos soltos) 
                // e garante que o tamanho máximo aceito pela regra de negócio seja exatamente 
                // o tamanho da coluna varchar(254) gerada no SQL Server.
                tf.Property(c => c.Endereco)
                    .IsRequired()
                    .HasColumnName("Email")
                    .HasColumnType($"varchar({Email.EmailMaxEndereco})");
            });

            // Mapeia o Value Object Cpf
            builder.OwnsOne(c => c.Cpf, tf =>
            {
                // REVISÃO (DRY): O mesmo conceito do e-mail se aplica aqui.
                // O Cpf.CpfMaxNumero (geralmente 11) define o tamanho do CPF na regra
                // e no banco simultaneamente.
                tf.Property(c => c.Numero)
                    .IsRequired()
                    .HasMaxLength(Cpf.CpfMaxNumero)
                    .HasColumnName("Cpf")
                    .HasColumnType($"varchar({Cpf.CpfMaxNumero})");
            });

            // Mapeia o Value Object Endereco (Composto por vários campos)
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

            // Define explicitamente o nome da tabela física no SQL Server
            builder.ToTable("Clientes");
        }
    }
}