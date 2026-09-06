using NME.Catalogo.API.Models;
using NME.Catalogo.API.Service;

namespace NME.Catalogo.API.Services
{
    public class ProductAppService : IProductAppService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProductAppService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<Produto>> ObterTodos()
        {
            return await _produtoRepository.ObterTodos();
        }

        public async Task<Produto?> ObterPorId(Guid id)
        {
            return await _produtoRepository.ObterPorId(id);
        }

        public void Dispose()
        {
            _produtoRepository?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}