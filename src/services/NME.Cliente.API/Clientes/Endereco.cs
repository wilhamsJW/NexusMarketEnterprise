namespace NME.Cliente.API.Clientes
{
    public class Endereco
    {
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Complemento { get; private set; }
        public string Bairro { get; private set; }
        public string Cep { get; private set; }
        public string Cidade { get; private set; }
        public string Estado { get; private set; }

        // Chave estrangeira para o EF Core
        public Guid ClienteId { get; private set; }

        protected Endereco() {
            Logradouro = null!;
            Numero = null!;
            Complemento = null!;
            Bairro = null!;
            Cep = null!;
            Cidade = null!;
            Estado = null!;
        }

        public Endereco(string logradouro, string numero, string complemento, string bairro, string cep, string cidade, string estado, Guid clienteId)
        {
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cep = cep;
            Cidade = cidade;
            Estado = estado;
            ClienteId = clienteId;
        }
    }
}