using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NME.Cliente.API.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    Cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "varchar(200)", nullable: true),
                    Endereco_Numero = table.Column<string>(type: "varchar(50)", nullable: true),
                    Endereco_Complemento = table.Column<string>(type: "varchar(250)", nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "varchar(100)", nullable: true),
                    Endereco_Cep = table.Column<string>(type: "varchar(20)", nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "varchar(100)", nullable: true),
                    Endereco_Estado = table.Column<string>(type: "varchar(50)", nullable: true),
                    Endereco_ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
