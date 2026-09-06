using NME.Catalogo.API.Models;

namespace NME.Catalogo.API.Service
{
    public interface IProductAppService : IDisposable
    {
        Task<IEnumerable<Produto>> ObterTodos();
        Task<Produto?> ObterPorId(Guid id);
    }
}