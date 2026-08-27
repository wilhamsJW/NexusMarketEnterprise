namespace NME.Identidade.API.Models
{
    public class UsuarioRespostaLogin
    {
        public string AccessToken { get; set; } = string.Empty;
        public double ExpiresIn { get; set; }
        public UsuarioToken UsuarioToken { get; set; } = new UsuarioToken();
    }

    public class UsuarioToken
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<UsuarioClaim> Claims { get; set; } = new List<UsuarioClaim>();
    }

    public class UsuarioClaim
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}