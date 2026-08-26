using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NME.Identidade.API.Models;

namespace NME.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/identidade")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new IdentityUser
            {
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true
            };

            // Cria o usuário no banco aplicando o Hash da senha automaticamente
            var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            {
                // Faz o login automático após o registro (opcional nessa etapa)
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("autenticar")]
        public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Valida o e-mail e a senha digitados
            var result = await _signInManager.PasswordSignInAsync(
                usuarioLogin.Email,
                usuarioLogin.Senha,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest("Usuário ou Senha incorretos");
        }
    }
}