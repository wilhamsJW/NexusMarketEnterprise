using System.Text.RegularExpressions;

namespace NME.Core.Utils
{
    public static class StringUtils
    {
        public static string ApenasNumeros(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            // Remove qualquer caractere que NÃO seja número
            return Regex.Replace(str, @"[^\d]", "");
        }
    }
}