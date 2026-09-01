using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NME.Identidade.API.Data;
using NME.Identidade.API.Models;

namespace NME.Identidade.API.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            JwtService jwtService,
            ApplicationDbContext context,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        public async Task<ResultadoOperacao<UsuarioRespostaLogin>> RegistrarUsuarioAsync(UsuarioRegistro usuarioRegistro)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                var user = new IdentityUser
                {
                    UserName = usuarioRegistro.Email,
                    Email = usuarioRegistro.Email,
                    EmailConfirmed = true
                };

                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Criar usuário no Identity
                    var resultCriacao = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

                    if (!resultCriacao.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return ResultadoOperacao<UsuarioRespostaLogin>.CriarFalha(
                            resultCriacao.Errors.Select(e => e.Description).ToArray());
                    }

                    // Gerar token JWT
                    UsuarioRespostaLogin respostaLogin;
                    try
                    {
                        respostaLogin = await _jwtService.GerarJwtAsync(user.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao gerar token JWT para o usuário {Email}. Revertendo criação do usuário.", user.Email);

                        await transaction.RollbackAsync();
                        await _userManager.DeleteAsync(user);

                        return ResultadoOperacao<UsuarioRespostaLogin>.CriarFalha(
                            "Erro ao gerar token de autenticação. Por favor, tente novamente.");
                    }

                    // Commit da transação
                    await transaction.CommitAsync();

                    return ResultadoOperacao<UsuarioRespostaLogin>.CriarSucesso(respostaLogin);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao registrar usuário {Email}", user.Email);

                    await transaction.RollbackAsync();

                    // Tentar limpar o usuário se foi criado
                    await TentarRemoverUsuarioAsync(user.Email);

                    return ResultadoOperacao<UsuarioRespostaLogin>.CriarFalha("Erro interno ao processar o registro. Por favor, tente novamente.");
                }
            });
        }

        public async Task<ResultadoOperacao<UsuarioRespostaLogin>> AutenticarUsuarioAsync(UsuarioLogin usuarioLogin)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    usuarioLogin.Email,
                    usuarioLogin.Senha,
                    isPersistent: false,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    try
                    {
                        var respostaLogin = await _jwtService.GerarJwtAsync(usuarioLogin.Email);
                        return ResultadoOperacao<UsuarioRespostaLogin>.CriarSucesso(respostaLogin);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao gerar token JWT para o usuário {Email} durante o login", usuarioLogin.Email);

                        return ResultadoOperacao<UsuarioRespostaLogin>.CriarFalha(
                            "Erro ao gerar token de autenticação. Por favor, tente novamente.");
                    }
                }

                if (result.IsLockedOut)
                {
                    return ResultadoOperacao<UsuarioRespostaLogin>.CriarFalha(
                        "Usuário temporariamente bloqueado por tentativas inválidas.");
                }

                return ResultadoOperacao<UsuarioRespostaLogin>.CriarFalha(
                    "Usuário ou senha incorretos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao autenticar usuário {Email}", usuarioLogin.Email);

                return ResultadoOperacao<UsuarioRespostaLogin>.CriarFalha(
                    "Erro interno ao processar a autenticação. Por favor, tente novamente.");
            }
        }

        private async Task TentarRemoverUsuarioAsync(string email)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    await _userManager.DeleteAsync(existingUser);
                }
            }
            catch (Exception cleanupEx)
            {
                _logger.LogError(cleanupEx, "Erro ao limpar usuário {Email} após falha no registro", email);
            }
        }
    }
}