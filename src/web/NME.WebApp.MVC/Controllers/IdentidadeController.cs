using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NME.WebApp.MVC.Models;

namespace NME.WebApp.MVC.Controllers
{
    [Route("")]
    public class IdentidadeController : Controller
    {
        // GET: Exibe a tela de cadastro vazia para o usuário
        [HttpGet("nova-conta")]
        public IActionResult Registro()
        {
            return View();
        }

        // POST: Recebe os dados do formulário e tenta cadastrar
        [HttpPost("nova-conta")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(UsuarioRegistro usuarioRegistro)
        {
            // Retorna para a tela se houver erro de validação (ex: e-mail inválido)
            if (!ModelState.IsValid) return View(usuarioRegistro);

            await Task.CompletedTask;

            // Redireciona o usuário para a página inicial após registrar
            return RedirectToAction("Index", "Home");
        }

        // GET: Exibe a tela de login vazia
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Recebe e-mail/senha e tenta autenticar
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return View(usuarioLogin);

            await Task.CompletedTask;

            return RedirectToAction("Index", "Home");
        }

        // GET: Apenas executa a saída (limpa o cookie) e redireciona. Não precisa de tela.
        [HttpGet("sair")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}