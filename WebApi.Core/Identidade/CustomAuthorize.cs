using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Core.Identidade
{
    // =========================================================================
    // PASSO 1: O PONTO DE ENTRADA (O Atributo que você coloca no Controller)
    // =========================================================================
    // É isso que você escreve sobre o método do Controller: [ClaimsAuthorize("Catalogo", "Ler")]
    // Quando a API sobe, o C# lê esse atributo primeiro.

    // Herda de TypeFilterAttribute porque atributos normais do C# não aceitam injeção de dependência nem instanciação
    // dinâmica por padrão.
    // O TypeFilterAttribute funciona como uma "ponte" entre a anotação[...] na sua Controller e a execução real da
    // classe RequisitoClaimFilter pelo container do ASP.NET Core.
    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        // O construtor recebe as duas strings que você digitou no Controller: "Catalogo" e "Ler"
        public ClaimsAuthorizeAttribute(string claimName, string claimValue)
            : base(typeof(RequisitoClaimFilter)) // <- "Avisa" o ASP.NET: "Quem vai executar a lógica é a classe do PASSO 2!"
        {
            // O Arguments repassa os dados ("Catalogo", "Ler") transformados em um objeto Claim 
            // para o construtor da classe do PASSO 2 (RequisitoClaimFilter).
            Arguments = new object[] { new Claim(claimName, claimValue) };
        }
    }

    // =========================================================================
    // PASSO 2: O INTERCEPTADOR DA REQUISIÇÃO (O Filtro de Autorização)
    // =========================================================================
    // Esta classe herda de 'IAuthorizationFilter'. 
    // No ASP.NET, qualquer classe que herda dessa interface vira um "pedágio" antes da Controller.
    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        private readonly Claim _claim; // Guarda a claim exigida ("Catalogo", "Ler")

        // 1. O ASP.NET constrói essa classe injetando a 'Claim' que veio do PASSO 1 no 'Arguments'.
        public RequisitoClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        // 2. O método 'OnAuthorization' é executado AUTOMATICAMENTE pelo ASP.NET quando a requisição chega.
        // O parâmetro 'context' (AuthorizationFilterContext) é entregue pelo próprio ASP.NET e contém TUDO
        // sobre a requisição atual, incluindo o HttpContext (onde está o usuário e os dados do Token).
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // A. Checa se o usuário NÃO está autenticado no HttpContext entregue pelo framework
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                // Se não enviou Token válido: altera o result para 401 e interrompe o fluxo
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            // B. Se está autenticado, repassa a validação final para o PASSO 3 (A regra pura)
            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value))
            {
                // Se não tem a Claim exigida: altera o result para 403 (Proibido)
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            // Se nada for alterado em 'context.Result', o ASP.NET entende que a requisição está LIBERADA 
            // e finalmente executa o método do seu Controller.
        }
    }

    // =========================================================================
    // PASSO 3: A REGRA DE NEGÓCIO PURA (A Classe Utilitária de Validação)
    // =========================================================================
    // Esta classe contém apenas o cálculo lógico booleano (Sim ou Não).
    public class CustomAuthorization
    {
        public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
        {
            // Recebe o HttpContext vindo do PASSO 2, lê as claims que o JwtBearer decodificou do Token 
            // e verifica se o usuário possui 'Type' == "Catalogo" e 'Value' contendo "Ler".
            return (context.User.Identity?.IsAuthenticated ?? false) &&
                   context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
        }
    }
}