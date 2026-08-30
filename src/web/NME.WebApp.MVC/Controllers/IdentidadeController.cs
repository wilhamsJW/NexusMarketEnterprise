using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NME.WebApp.MVC.Models;
using NME.WebApp.MVC.Services;

namespace NME.WebApp.MVC.Controllers
{
    [Route("")]
    public class IdentidadeController : Controller
    {
        private readonly IAutenticacaoService _autenticacaoService;

        public IdentidadeController(IAutenticacaoService autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }

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

            var resposta = await _autenticacaoService.Registro(usuarioRegistro);

            if (ResponsePossuiErros(resposta)) return View(usuarioRegistro);

            await _autenticacaoService.RealizarLogin(resposta);

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

            var resposta = await _autenticacaoService.Login(usuarioLogin);

            if (ResponsePossuiErros(resposta)) return View(usuarioLogin);

            await _autenticacaoService.RealizarLogin(resposta);

            return RedirectToAction("Index", "Home");
        }

        // GET: Apenas executa a saída (limpa o cookie) e redireciona. Não precisa de tela.
        [HttpGet("sair")]
        public async Task<IActionResult> Logout()
        {
            // Remove o cookie de sessão antes do redirect, evitando usuário "fantasma" autenticado
            await _autenticacaoService.Logout();

            return RedirectToAction("Index", "Home");
        }

        private bool ResponsePossuiErros(UsuarioRespostaLogin resposta)
        {
            if (resposta.Erros is null || !resposta.Erros.Any()) return false;

            foreach (var erro in resposta.Erros)
            {
                ModelState.AddModelError(string.Empty, erro);
            }

            return true;
        }
    }
}