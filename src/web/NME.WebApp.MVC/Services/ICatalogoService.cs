using NME.WebApp.MVC.Models;

namespace NME.WebApp.MVC.Services;
public interface ICatalogoService
{
    Task<IEnumerable<ProdutoViewModel>> ObterTodos();
    Task<ProdutoViewModel?> ObterPorId(Guid id);
}