using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NME.Identidade.API.Data;
using NME.Identidade.API.Models;
using NME.Identidade.API.Services;

namespace NME.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/identidade")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            JwtService jwtService,
            ApplicationDbContext context,
            ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("registrar")]
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

            var user = new IdentityUser
            {
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true
            };

            // Iniciar transação para garantir atomicidade
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Criar usuário
                var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new
                    {
                        success = false,
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                // Gerar JWT
                UsuarioRespostaLogin resposta;
                try
                {
                    resposta = await _jwtService.GerarJwtAsync(user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao gerar token JWT para o usuário {Email}. Revertendo criação do usuário.", user.Email);
                    
                    // Reverter a transação
                    await transaction.RollbackAsync();
                    
                    // Remover usuário criado
                    await _userManager.DeleteAsync(user);

                    return StatusCode(500, new
                    {
                        success = false,
                        errors = new[] { "Erro ao gerar token de autenticação. Por favor, tente novamente." }
                    });
                }

                // Commit da transação apenas se tudo ocorreu com sucesso
                await transaction.CommitAsync();

                return Ok(resposta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar usuário {Email}", user.Email);
                
                await transaction.RollbackAsync();

                // Tentar limpar o usuário se foi criado
                try
                {
                    var existingUser = await _userManager.FindByEmailAsync(user.Email);
                    if (existingUser != null)
                    {
                        await _userManager.DeleteAsync(existingUser);
                    }
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Erro ao limpar usuário após falha no registro");
                }

                return StatusCode(500, new
                {
                    success = false,
                    errors = new[] { "Erro interno ao processar o registro. Por favor, tente novamente." }
                });
            }
        }

        [HttpPost("autenticar")]
        [HttpPost("login")]
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

            var result = await _signInManager.PasswordSignInAsync(
                usuarioLogin.Email,
                usuarioLogin.Senha,
                isPersistent: false,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                try
                {
                    var resposta = await _jwtService.GerarJwtAsync(usuarioLogin.Email);
                    return Ok(resposta);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao gerar token JWT para o usuário {Email} durante o login", usuarioLogin.Email);
                    
                    return StatusCode(500, new
                    {
                        success = false,
                        errors = new[] { "Erro ao gerar token de autenticação. Por favor, tente novamente." }
                    });
                }
            }

            if (result.IsLockedOut)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { "Usuário temporariamente bloqueado por tentativas inválidas." }
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = new[] { "Usuário ou senha incorretos." }
            });
        }
    }
}