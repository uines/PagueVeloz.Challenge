using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Challenge.Application.Commands.Cliente;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Handlers;
using PagueVeloz.Challenge.Application.Queries.Conta;
using PagueVeloz.Challenge.Application.Queries.Transacao; 

namespace PagueVeloz.Challenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly CriarClienteCommandHandler _criarClienteHandler;
        private readonly GetSaldoContaQueryHandler _getSaldoContaQueryHandler; 
        private readonly GetHistoricoTransacoesQueryHandler _getHistoricoTransacoesQueryHandler;

        public ClientesController(
            CriarClienteCommandHandler criarClienteHandler,
            GetSaldoContaQueryHandler getSaldoContaQueryHandler,
            GetHistoricoTransacoesQueryHandler getHistoricoTransacoesQueryHandler)
        {
            _criarClienteHandler = criarClienteHandler;
            _getSaldoContaQueryHandler = getSaldoContaQueryHandler;
            _getHistoricoTransacoesQueryHandler = getHistoricoTransacoesQueryHandler;
        }

        /// <summary>
        /// Cria um novo cliente e sua conta associada.
        /// </summary>
        /// <param name="dto">Dados para a criação do cliente.</param>
        /// <returns>O cliente e a conta criados.</returns>
        [HttpPost] 
        [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status201Created)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status409Conflict)] 
        public async Task<IActionResult> CriarCliente([FromBody] CriarClienteDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new CriarClienteCommand(dto);
                var clienteCriado = await _criarClienteHandler.Handle(command);

                return CreatedAtAction(nameof(GetClienteById), new { id = clienteCriado.Id }, clienteCriado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao criar o cliente.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um cliente pelo seu ID.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>O cliente encontrado.</returns>
        [HttpGet("{id}")] 
        [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClienteById(Guid id)
        {
            // Por enquanto, apenas um stub. Você precisaria de um GetClienteByIdQuery e seu handler.
            // var cliente = await _getClienteByIdQueryHandler.Handle(new GetClienteByIdQuery(id));
            // if (cliente == null)
            // {
            //     return NotFound();
            // }
            // return Ok(cliente);

            return StatusCode(StatusCodes.Status501NotImplemented, "Este endpoint ainda não foi implementado. Foco no desafio principal.");
        }

        /// <summary>
        /// Obtém o saldo de uma conta específica de um cliente.
        /// </summary>
        /// <param name="clienteId">ID do cliente cuja conta será consultada.</param>
        /// <returns>O saldo detalhado da conta.</returns>
        [HttpGet("{clienteId}/conta/saldo")]
        [ProducesResponseType(typeof(SaldoContaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSaldoConta(Guid clienteId)
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Por favor, use o endpoint de saldo no /api/contas/{id}/saldo que será criado.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao consultar o saldo.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o histórico de transações de uma conta específica de um cliente.
        /// </summary>
        /// <param name="clienteId">ID do cliente cuja conta será consultada.</param>
        /// <param name="dataInicio">Data de início para filtrar transações (opcional).</param>
        /// <param name="dataFim">Data de fim para filtrar transações (opcional).</param>
        /// <param name="skip">Número de registros a pular (para paginação).</param>
        /// <param name="take">Número de registros a retornar (para paginação).</param>
        /// <returns>Uma lista de transações.</returns>
        [HttpGet("{clienteId}/conta/historico")] 
        [ProducesResponseType(typeof(IEnumerable<TransacaoResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHistoricoTransacoes(
            Guid clienteId, 
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Por favor, use o endpoint de histórico no /api/contas/{id}/historico que será criado.");
        }
    }
}