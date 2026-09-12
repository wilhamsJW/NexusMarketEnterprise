using System.Net.Http.Headers;
using NME.WebApp.MVC.Interfaces;

namespace NME.WebApp.MVC.Services.Handlers;

/// <summary>
/// Interceptador de requisições HTTP de saída (DelegatingHandler).
/// Atua como um middleware do HttpClient para injetar automaticamente o Token JWT 
/// do usuário logado no cabeçalho 'Authorization' antes de enviar a chamada para as APIs de backend.
/// </summary>
public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    // Dependência responsável por prover as informações do usuário logado na sessão atual
    private readonly IUser _user;

    /// <summary>
    /// Construtor que recebe as dependências via Injeção de Dependência (DI).
    /// </summary>
    /// <param name="user">Interface de abstração do usuário autenticado (geralmente baseada no HttpContextAccessor)</param>
    public HttpClientAuthorizationDelegatingHandler(IUser user)
    {
        _user = user;
    }

    /// <summary>
    /// Método interceptador executado em todas as chamadas HTTP efetuadas pelo HttpClient associado.
    /// </summary>
    /// <param name="request">A mensagem da requisição HTTP de saída</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona</param>
    /// <returns>A resposta da requisição HTTP recebida da API de destino</returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 1. Obtém o Token JWT do usuário autenticado no MVC através do serviço IUser
        var token = _user.ObterUserToken();

        // 2. Se o token existir no contexto atual, injeta o cabeçalho "Authorization: Bearer <token>"
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 3. Transmite a requisição (já com o token anexado) para o próximo Handler do pipeline ou para a rede
        return await base.SendAsync(request, cancellationToken);
    }
}