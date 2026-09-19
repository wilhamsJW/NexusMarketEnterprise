

using NME.Core.DomainObjects;

namespace NME.Cliente.API.Clientes
{
    public class Cliente : Entity, IAggregateRoot
    {
        // Propriedades com private set para garantir o encapsulamento
        public string Nome { get; private set; }
        public Email Email { get; private set; } 
        public Cpf Cpf { get; private set; }
        public bool Excluido { get; private set; }
        public Endereco? Endereco { get; private set; }

        // Construtor protegido exigido pelo Entity Framework Core
        protected Cliente()
        {
            Nome = null!;
            Email = null!;
            Cpf = null!;
        }

        // Construtor principal para criação de um novo Cliente válido
        public Cliente(string nome, Email email, Cpf cpf)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Excluido = false;
        }

        // Método de negócio para atribuir/atualizar o Endereço via Raiz de Agregação
        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        // Método de negócio para exclusão lógica
        public void Excluir()
        {
            Excluido = true;
        }
    }
}
