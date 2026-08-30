using NME.WebApp.MVC.Models;

namespace NME.WebApp.MVC.Services
{
    public interface IAutenticacaoService
    {
        Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin);

        Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro);

        Task RealizarLogin(UsuarioRespostaLogin resposta);

        Task Logout();
    }
}
