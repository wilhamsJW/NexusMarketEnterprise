using Microsoft.Extensions.Options;
using NME.Core;
using NME.WebApp.MVC.Interfaces;
using NME.WebApp.MVC.Providers;
using NME.WebApp.MVC.Services;

namespace NME.WebApp.MVC.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // AddHttpContextAccessor: Permite acessar o HttpContext atual em classes que não são Controllers (ex: serviços)
        services.AddHttpContextAccessor();

        // AddScoped: Cria uma instância de AspNetUser por requisição HTTP para extrair dados do usuário logado
        services.AddScoped<IUser, AspNetUser>();

        // AddHttpClient: Registra o serviço IAutenticacaoService usando um HttpClient fortemente tipado
        // O parâmetro 'provider' (IServiceProvider) é o container do .NET usado para resolver dependências já registradas
        services.AddHttpClient<IAutenticacaoService, AutenticacaoService>((provider, client) =>
        {
            // IOptions<AppSettings>: Padrão do .NET para ler a seção "AppSettings" do arquivo appsettings.json de forma tipada
            // .Value: Extrai o objeto AppSettings preenchido com as propriedades
            var appSettings = provider.GetRequiredService<IOptions<AppSettings>>().Value;

            // Valida se a URL base da API de Identidade foi configurada no appsettings.json
            var identidadeUrl = appSettings.AutenticacaoUrl
                ?? throw new InvalidOperationException("Configuração 'AutenticacaoUrl' não definida em AppSettings.");

            // Adiciona a barra "/" no final se não houver, para evitar erro de montagem de rota no HttpClient
            if (!identidadeUrl.EndsWith('/')) identidadeUrl += "/";

            // Define o endereço base padrão (ex: https://localhost:5001/) para todas as chamadas feitas por este serviço
            client.BaseAddress = new Uri(identidadeUrl);

            // Tempo limite da conexão antes de estourar timeout no HttpClient
            client.Timeout = TimeSpan.FromSeconds(60);
        })
        // AddStandardResilienceHandler: Adiciona políticas nativas de resiliência do Polly (Retry, Circuit Breaker, Timeout)
        .AddStandardResilienceHandler();

        // Cliente HTTP para consumir a API de Catálogo
        services.AddHttpClient<ICatalogoService, CatalogoService>((provider, client) =>
        {
            // Resolve o IOptions<AppSettings> cadastrado para pegar as URLs de configuração
            var appSettings = provider.GetRequiredService<IOptions<AppSettings>>().Value;

            // Valida se a URL base da API de Catálogo foi configurada no appsettings.json
            var catalogoUrl = appSettings.CatalogoUrl
                ?? throw new InvalidOperationException("Configuração 'CatalogoUrl' não definida em AppSettings.");

            // Adiciona a barra "/" no final da URL caso não exista
            if (!catalogoUrl.EndsWith('/')) catalogoUrl += "/";

            // Define o endereço base para as requisições de produtos
            client.BaseAddress = new Uri(catalogoUrl);

            // Configura o tempo máximo de espera do cliente HTTP
            client.Timeout = TimeSpan.FromSeconds(60);
        })
        // Aplica resiliência e tratamento de falhas temporárias do Polly
        .AddStandardResilienceHandler();
    }
}