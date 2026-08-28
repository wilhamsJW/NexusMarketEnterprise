using Microsoft.AspNetCore.Mvc;
using NME.Identidade.API.Models;
using NME.Identidade.API.Services;

namespace NME.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/identidade")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="usuarioRegistro">Dados de registro do usuário</param>
        /// <returns>Token JWT e informações do usuário</returns>
        [HttpPost("registrar")]
        [ProducesResponseType(typeof(UsuarioRespostaLogin), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UsuarioRespostaLogin>> Registrar(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var resultado = await _authService.RegistrarUsuarioAsync(usuarioRegistro);

            if (!resultado.Sucesso)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = resultado.Erros
                });
            }

            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Autentica um usuário existente
        /// </summary>
        /// <param name="usuarioLogin">Credenciais de login</param>
        /// <returns>Token JWT e informações do usuário</returns>
        [HttpPost("autenticar")]
        [HttpPost("login")]
        [ProducesResponseType(typeof(UsuarioRespostaLogin), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UsuarioRespostaLogin>> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var resultado = await _authService.AutenticarUsuarioAsync(usuarioLogin);

            if (!resultado.Sucesso)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = resultado.Erros
                });
            }

            return Ok(resultado.Dados);
        }
    }
}