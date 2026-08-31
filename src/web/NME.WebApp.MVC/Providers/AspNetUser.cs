using NME.WebApp.MVC.Interfaces;
using System.Security.Claims;

namespace NME.WebApp.MVC.Providers
{
    // Classe concreta responsável por prover os dados do usuário a partir do contexto HTTP
    public class AspNetUser : IUser
    {
        // Acesso ao HttpContext do ASP.NET Core via injeção de dependência
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        // Propriedade privada que busca a identidade (ClaimsPrincipal) contida no HttpContext da requisição atual
        private ClaimsPrincipal? Usuario => _accessor.HttpContext?.User;

        // Verifica se a requisição possui um usuário autenticado no cookie de sessão
        public bool Autenticado()
        {
            return Usuario?.Identity?.IsAuthenticated ?? false;
        }

        // Extrai o ID do usuário através da Claim de NameIdentifier
        public string ObterUserId()
        {
            return Autenticado()
                ? Usuario?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty
                : string.Empty;
        }

        // Extrai o e-mail do usuário da Claim de Email (ou recai sobre o Identity.Name caso a claim falhe)
        public string ObterUserEmail()
        {
            if (!Autenticado()) return string.Empty;

            return Usuario?.FindFirst(ClaimTypes.Email)?.Value
                ?? Usuario?.Identity?.Name
                ?? string.Empty;
        }

        // Extrai o token JWT original da API que ficou salvo dentro da Claim personalizada "JWT" do Cookie
        public string ObterUserToken()
        {
            return Autenticado()
                ? Usuario?.FindFirst("JWT")?.Value ?? string.Empty
                : string.Empty;
        }

        // Retorna a lista completa de Claims associadas ao usuário atual na requisição
        public IEnumerable<Claim> ObterClaims()
        {
            return Usuario?.Claims ?? Enumerable.Empty<Claim>();
        }
    }
}