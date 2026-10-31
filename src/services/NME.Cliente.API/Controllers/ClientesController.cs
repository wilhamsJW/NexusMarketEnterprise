using Microsoft.AspNetCore.Mvc;
using NME.Cliente.API.DTOs;
using NME.Clientes.API.Application.Commands;
using NME.Core.Mediator; // <-- Importante: Namespace do nosso barramento customizado

namespace NME.Cliente.API.Controllers
{
    /// <summary>
    /// OBJETIVO: Controladora responsável por gerenciar os endpoints HTTP da entidade Cliente.
    /// Padrão: REST API / Controller Pattern.
    /// </summary>
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        // Variável privada que guarda o nosso barramento customizado (IMediatorHandler)
        private readonly IMediatorHandler _mediatorHandler;

        // INJEÇÃO DE DEPENDÊNCIA: O .NET entrega automaticamente a implementação do nosso MediatorHandler aqui
        public ClientesController(IMediatorHandler mediatorHandler)
        {
            _mediatorHandler = mediatorHandler;
        }

        /// <summary>
        /// Endpoint responsável por receber a requisição HTTP POST para cadastrar um novo cliente.
        /// </summary>
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarClienteDto model)
        {
            // 1. Mapeia os dados brutos que vieram do DTO (do front-end/Swagger) 
            // transformando-os em um Command oficial do nosso domínio.
            var command = new RegistrarClienteCommand(model.Id, model.Nome, model.Email, model.Cpf);

            // 2. DISPACHA VIA BARRAMENTO CUSTOMIZADO: 
            // Em vez de chamar o MediatR puro, usamos o nosso IMediatorHandler. 
            // O Handler lá no fundo vai processar as regras, validar e retornar o resultado.
            var validationResult = await _mediatorHandler.EnviarComando(command);

            // 3. TRATAMENTO DE ERROS: 
            // Se houver falhas de validação (FluentValidation), pegamos as mensagens de erro 
            // e retornamos um HTTP 400 (BadRequest) limpo para quem chamou.
            if (!validationResult.IsValid)
            {
                var erros = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { erros });
            }

            // 4. SUCESSO: 
            // Se passou por todas as regras sem erros, retorna um HTTP 200 (Ok) com a mensagem de sucesso.
            return Ok(new { mensagem = "Cliente registrado com sucesso!" });
        }
    }
}

/*
====================================================================================================
POR BAIXO DOS PANOS (Mecanismos C# e Controllers)
====================================================================================================

- Inversão de Dependência Avançada:
  Agora a nossa Controller não sabe o que é o MediatR e nem o que é a biblioteca de terceiros. 
  Ela só conhece o nosso contrato (IMediatorHandler). Se amanhã quisermos trocar o MediatR por 
  outra ferramenta de barramento, a ClientesController não precisa ser alterada em absolutamente nada! 
  Isso é o ápice do desacoplamento em arquitetura enterprise.

- O método _mediatorHandler.EnviarComando(command):
  Quando a Controller executa essa linha, o C# aciona o método genérico <T> que criamos na classe 
  MediatorHandler. O modificador await pausa a execução da API de forma assíncrona (liberando a thread 
  para atender outras requisições enquanto o banco processa) até que o ClienteCommandHandler termine 
  de rodar e devolva o ValidationResult.
====================================================================================================
*/