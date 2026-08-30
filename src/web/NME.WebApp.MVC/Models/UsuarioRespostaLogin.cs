using System.Text.Json.Serialization;

namespace NME.WebApp.MVC.Models
{
    public class UsuarioRespostaLogin
    {
        public string AccessToken { get; set; } = string.Empty;
        public double ExpiresIn { get; set; }
        public UsuarioToken UsuarioToken { get; set; } = new UsuarioToken();

        [JsonPropertyName("errors")]
        public IEnumerable<string> Erros { get; set; } = Enumerable.Empty<string>();
    }

    public class UsuarioToken
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<UsuarioClaim> Claims { get; set; } = Enumerable.Empty<UsuarioClaim>();
    }

    public class UsuarioClaim
    {
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}