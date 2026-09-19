using System.Text.RegularExpressions;

namespace NME.Core.DomainObjects
{
    public class Email
    {
        public const int EmailMaxEndereco = 254;
        public const int EmailMinEndereco = 5;
        public string Endereco { get; private set; }

        // Construtor protegido exigido pelo Entity Framework Core
        protected Email()
        {
            Endereco = null!;
        }

        public Email(string endereco)
        {
            if (!Validar(endereco))
                throw new DomainException("E-mail inválido.");

            Endereco = endereco;
        }

        public static bool Validar(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            email = email.Trim();

            if (email.Length < EmailMinEndereco || email.Length > EmailMaxEndereco)
                return false;

            var regexEmail = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)[0-9a-z]@))(?:\[(?:\d{1,3}\.){3}\d{1,3}\])|(?:(?:[0-9a-z][\w-]*[0-9a-z]*\.)+[a-z0-9][\w-]{0,22}[a-z0-9])$", RegexOptions.IgnoreCase);

            return regexEmail.IsMatch(email);
        }
    }
}