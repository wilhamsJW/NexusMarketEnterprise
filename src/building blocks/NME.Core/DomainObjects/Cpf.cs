using NME.Core.Utils;

namespace NME.Core.DomainObjects
{
    public class Cpf
    {
        public const int CpfMaxNumero = 11;
        public string Numero { get; private set; }

        // Construtor protegido exigido pelo Entity Framework Core
        protected Cpf()
        {
            Numero = null!;
        }

        public Cpf(string numero)
        {
            var cpfLimpo = numero.ApenasNumeros();

            if (!Validar(cpfLimpo))
                throw new DomainException("CPF inválido.");

            Numero = cpfLimpo;
        }

        public static bool Validar(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            // Remove formatação caso a string venha bruta
            cpf = cpf.ApenasNumeros();

            if (cpf.Length != CpfMaxNumero) return false;

            // Bloqueia CPFs com todos os dígitos iguais (ex: 111.111.111-11)
            if (TudoDigitoIgual(cpf)) return false;

            // Algoritmo Oficial de Verificação dos Dígitos
            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * (10 - i);

            var resto = soma % 11;
            var primeiroDigito = resto < 2 ? 0 : 11 - resto;

            tempCpf += primeiroDigito;
            soma = 0;

            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * (11 - i);

            resto = soma % 11;
            var segundoDigito = resto < 2 ? 0 : 11 - resto;

            return cpf.EndsWith(primeiroDigito.ToString() + segundoDigito.ToString());
        }

        private static bool TudoDigitoIgual(string cpf)
        {
            var primeiroDigito = cpf[0];
            for (var i = 1; i < cpf.Length; i++)
            {
                if (cpf[i] != primeiroDigito) return false;
            }
            return true;
        }
    }
}