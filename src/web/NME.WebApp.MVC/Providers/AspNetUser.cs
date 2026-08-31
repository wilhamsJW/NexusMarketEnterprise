using NME.WebApp.MVC.Interfaces;
using System.Security.Claims;

namespace NME.WebApp.MVC.Providers
{
    public class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        private ClaimsPrincipal? Usuario => _accessor.HttpContext?.User;

        public bool Autenticado()
        {
            return Usuario?.Identity?.IsAuthenticated ?? false;
        }

        public string ObterUserId()
        {
            return Autenticado()
                ? Usuario?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty
                : string.Empty;
        }

        public string ObterUserEmail()
        {
            if (!Autenticado()) return string.Empty;

            return Usuario?.FindFirst(ClaimTypes.Email)?.Value
                ?? Usuario?.Identity?.Name
                ?? string.Empty;
        }

        public string ObterUserToken()
        {
            return Autenticado()
                ? Usuario?.FindFirst("JWT")?.Value ?? string.Empty
                : string.Empty;
        }

        public IEnumerable<Claim> ObterClaims()
        {
            return Usuario?.Claims ?? Enumerable.Empty<Claim>();
        }
    }
}
