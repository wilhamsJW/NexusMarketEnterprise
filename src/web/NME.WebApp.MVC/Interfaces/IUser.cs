using System.Security.Claims;

namespace NME.WebApp.MVC.Interfaces
{
    public interface IUser
    {
        bool Autenticado();

        string ObterUserId();

        string ObterUserEmail();

        string ObterUserToken();

        IEnumerable<Claim> ObterClaims();
    }
}
