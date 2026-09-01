using System.Net;
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

        // Status HTTP da chamada. Ignorado na desserialização; preenchido pelo AutenticacaoService.
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        // True para 5xx, timeout, circuito aberto ou falha de rede — dispara a troca de componente na UI.
        [JsonIgnore]
        public bool FalhaDeInfraestrutura { get; set; }
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