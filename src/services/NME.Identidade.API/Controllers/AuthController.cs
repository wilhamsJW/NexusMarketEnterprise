using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NME.Identidade.API.Configuration;
using NME.Identidade.API.Models;
using NME.Identidade.API.Services;

namespace NME.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/identidade")]
    // Passa a constante em vez de escrever a string manualmente "FixedWindow"
    [EnableRateLimiting(ApiConfig.FixedWindowPolicy)]
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
        [HttpPost("nova-conta")]
        [ProducesResponseType(typeof(UsuarioRespostaLogin), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
                return BadRequest(new { success = false, errors = resultado.Erros });
            }

            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Autentica um usuário existente
        /// </summary>
        [HttpPost("autenticar")]
        // Retorna os tokens JWT e dados do usuário quando a autenticação é bem-sucedida (200 OK)
        [ProducesResponseType(typeof(UsuarioRespostaLogin), StatusCodes.Status200OK)]
        // Retorna a lista de erros de validação caso os dados de login sejam inválidos (400 Bad Request)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // Retorna o aviso de bloqueio temporário caso o limite de tentativas por minuto seja excedido (429 Too Many Requests)
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
                return BadRequest(new { success = false, errors = resultado.Erros });
            }

            return Ok(resultado.Dados);
        }
    }
}