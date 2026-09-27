using MediatR;
using Microsoft.AspNetCore.Mvc;
using NME.Clientes.API.Application.Commands;

namespace NME.Cliente.API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(RegistrarClienteCommand command)
        {
            var resultado = await _mediator.Send(command);

            if (!resultado.IsValid)
            {
                return BadRequest(resultado.Errors);
            }

            return Ok();
        }
    }
}

//Por baixo dos panos (Mecanismos C# e Controllers)
//Baixo Acoplamento: A Controller não conhece o ClientesContext, o ClienteRepository
//    e nem as regras de validação. Ela depende unicamente da abstração IMediator para
//    despachar a intenção do usuário (RegistrarClienteCommand).

//Send(command): O MediatR localiza automaticamente o ClienteCommandHandler registrado 
//    na Injeção de Dependência e executa a lógica de cadastro, retornando o
//    ValidationResult do FluentValidation.
