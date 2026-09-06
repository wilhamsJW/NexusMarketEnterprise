using Microsoft.AspNetCore.Mvc;
using NME.Catalogo.API.Models;
using NME.Catalogo.API.Service;

namespace NME.Catalogo.API.Controllers
{
    [ApiController]
    [Route("api/catalogo")]
    public class ProductController : ControllerBase
    {
        private readonly IProductAppService _productAppService;

        public ProductController(IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        [HttpGet("produtos")]
        [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos()
        {
            var produtos = await _productAppService.ObterTodos();
            return Ok(produtos);
        }

        [HttpGet("produtos/{id:guid}")]
        [ProducesResponseType(typeof(Produto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            var produto = await _productAppService.ObterPorId(id);

            if (produto is null) return NotFound();

            return Ok(produto);
        }
    }
}
