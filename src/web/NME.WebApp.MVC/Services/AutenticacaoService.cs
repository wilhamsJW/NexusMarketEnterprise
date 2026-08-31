using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NME.WebApp.MVC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace NME.WebApp.MVC.Services
{
    public class AutenticacaoService : IAutenticacaoService
    {
        // Rotas relativas SEM barra inicial — obrigatório para concatenar com a BaseAddress
        private const string EndpointLogin = "api/identidade/autenticar";
        private const string EndpointRegistro = "api/identidade/nova-conta";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AutenticacaoService> _logger;

        public AutenticacaoService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AutenticacaoService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin)
        {
            return await EnviarRequisicaoAsync(EndpointLogin, usuarioLogin);
        }

        public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro)
        {
            return await EnviarRequisicaoAsync(EndpointRegistro, usuarioRegistro);
        }

        public async Task RealizarLogin(UsuarioRespostaLogin resposta)
        {
            ArgumentNullException.ThrowIfNull(resposta);

            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext indisponível.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, resposta.UsuarioToken.Id),
                new(ClaimTypes.Name, resposta.UsuarioToken.Email),
                new(ClaimTypes.Email, resposta.UsuarioToken.Email),
                new("JWT", resposta.AccessToken)
            };

            claims.AddRange(ObterClaimsDoToken(resposta.AccessToken));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(resposta.ExpiresIn),
                IsPersistent = false,
                AllowRefresh = true
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);
        }

        public async Task Logout()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null) return;

            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private async Task<UsuarioRespostaLogin> EnviarRequisicaoAsync<TRequest>(string endpointRegistro, TRequest payload)
        {
            try
            {
                using var content = new StringContent(
                    JsonSerializer.Serialize(payload, JsonOptions),
                    Encoding.UTF8,
                    "application/json");

                using var response = await _httpClient.PostAsync(endpointRegistro, content);

                var conteudo = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Falha na chamada a {Endpoint}. Status: {Status}",
                        endpointRegistro,
                        response.StatusCode);

                    return new UsuarioRespostaLogin
                    {
                        Erros = ExtrairErros(conteudo)
                    };
                }

                return JsonSerializer.Deserialize<UsuarioRespostaLogin>(conteudo, JsonOptions)
                    ?? new UsuarioRespostaLogin { Erros = ["Resposta inválida do serviço de identidade."] };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Serviço de identidade indisponível ao chamar {Endpoint}", endpointRegistro);

                return new UsuarioRespostaLogin
                {
                    Erros = ["Serviço de identidade indisponível. Tente novamente em instantes."]
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Falha ao desserializar a resposta de {Endpoint}", endpointRegistro);

                return new UsuarioRespostaLogin
                {
                    Erros = ["Não foi possível interpretar a resposta do servidor."]
                };
            }
        }

        private static IEnumerable<string> ExtrairErros(string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
            {
                return ["Ocorreu um erro ao processar a solicitação."];
            }

            try
            {
                using var document = JsonDocument.Parse(conteudo);

                if (document.RootElement.TryGetProperty("errors", out var errors) &&
                    errors.ValueKind == JsonValueKind.Array)
                {
                    return errors.EnumerateArray()
                                 .Select(e => e.GetString() ?? string.Empty)
                                 .Where(e => !string.IsNullOrWhiteSpace(e))
                                 .ToList();
                }
            }
            catch (JsonException)
            {
                // Resposta não é JSON válido — cai no retorno genérico abaixo.
            }

            return ["Ocorreu um erro ao processar a solicitação."];
        }

        private static IEnumerable<Claim> ObterClaimsDoToken(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt)) return [];

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(jwt)) return [];

            return handler.ReadJwtToken(jwt).Claims;
        }
    }
}