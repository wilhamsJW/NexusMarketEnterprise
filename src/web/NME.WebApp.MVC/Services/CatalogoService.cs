using NME.Core.Http;
using NME.WebApp.MVC.Models;

namespace NME.WebApp.MVC.Services;
public class CatalogoService : ICatalogoService
{
    private readonly HttpClient _httpClient;

    public CatalogoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
    {
        var response = await _httpClient.GetAsync("/api/catalogo/produtos");

        response.EnsureSuccessStatusCode();

        var produtos = await response.DeserializarObjetoResponseAsync<IEnumerable<ProdutoViewModel>>();

        return produtos ?? Enumerable.Empty<ProdutoViewModel>();
    }

    public async Task<ProdutoViewModel?> ObterPorId(Guid id)
    {
        var response = await _httpClient.GetAsync($"/api/catalogo/produtos/{id}");

        response.EnsureSuccessStatusCode();

        return await response.DeserializarObjetoResponseAsync<ProdutoViewModel>();
    }
}