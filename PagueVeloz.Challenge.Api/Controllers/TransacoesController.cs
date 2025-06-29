using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Challenge.Application.Commands.Transacao;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Handlers;
using PagueVeloz.Challenge.Application.Queries.Transacao;

namespace PagueVeloz.Challenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly RealizarVendaCommandHandler _realizarVendaHandler;
        private readonly RealizarEstornoCommandHandler _realizarEstornoHandler;
        private readonly RealizarTransferenciaCommandHandler _realizarTransferenciaHandler;
        private readonly GetTransacaoByIdQueryHandler _getTransacaoByIdQueryHandler;
        private readonly RealizarDepositoCommandHandler _realizarDepositoHandler;

        public TransacoesController(
            RealizarVendaCommandHandler realizarVendaHandler,
            RealizarEstornoCommandHandler realizarEstornoHandler,
            RealizarTransferenciaCommandHandler realizarTransferenciaHandler,
            GetTransacaoByIdQueryHandler getTransacaoByIdQueryHandler,
            RealizarDepositoCommandHandler realizarDepositoHandler)
        {
            _realizarVendaHandler = realizarVendaHandler;
            _realizarEstornoHandler = realizarEstornoHandler;
            _realizarTransferenciaHandler = realizarTransferenciaHandler;
            _getTransacaoByIdQueryHandler = getTransacaoByIdQueryHandler;
            _realizarDepositoHandler = realizarDepositoHandler;
        }

        /// <summary>
        /// Realiza uma operação de venda (débito, crédito à vista, crédito parcelado).
        /// </summary>
        /// <param name="dto">Dados da venda a ser realizada.</param>
        /// <returns>A transação de venda realizada.</returns>
        [HttpPost("venda")]
        [ProducesResponseType(typeof(TransacaoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RealizarVenda([FromBody] RealizarVendaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new RealizarVendaCommand(dto);
                var transacaoRealizada = await _realizarVendaHandler.Handle(command);
                return CreatedAtAction(nameof(GetTransacaoById), new { id = transacaoRealizada.Id }, transacaoRealizada);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não encontrada"))
                    return NotFound(new { message = ex.Message });

                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao realizar a venda.", error = ex.Message });
            }
        }

        /// <summary>
        /// Realiza o estorno de uma transação original.
        /// </summary>
        /// <param name="dto">Dados para o estorno.</param>
        /// <returns>A transação de estorno realizada.</returns>
        [HttpPost("estorno")] 
        [ProducesResponseType(typeof(TransacaoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [ProducesResponseType(StatusCodes.Status409Conflict)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RealizarEstorno([FromBody] RealizarEstornoDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new RealizarEstornoCommand(dto);
                var estornoRealizado = await _realizarEstornoHandler.Handle(command);
                return CreatedAtAction(nameof(GetTransacaoById), new { id = estornoRealizado.Id }, estornoRealizado);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não encontrada"))
                    return NotFound(new { message = ex.Message });

                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao realizar o estorno.", error = ex.Message });
            }
        }

        /// <summary>
        /// Realiza uma transferência entre contas.
        /// </summary>
        /// <param name="dto">Dados da transferência.</param>
        /// <returns>A transação de transferência realizada.</returns>
        [HttpPost("transferencia")] 
        [ProducesResponseType(typeof(TransacaoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [ProducesResponseType(StatusCodes.Status409Conflict)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RealizarTransferencia([FromBody] RealizarTransferenciaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new RealizarTransferenciaCommand(dto);
                var transferenciaRealizada = await _realizarTransferenciaHandler.Handle(command);
                return CreatedAtAction(nameof(GetTransacaoById), new { id = transferenciaRealizada.Id }, transferenciaRealizada);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não encontrada"))
                    return NotFound(new { message = ex.Message });

                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao realizar a transferência.", error = ex.Message });
            }
        }


        /// <summary>
        /// Obtém uma transação pelo seu ID.
        /// </summary>
        /// <param name="id">ID da transação.</param>
        /// <returns>A transação encontrada.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransacaoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransacaoById(Guid id)
        {
            try
            {
                var query = new GetTransacaoByIdQuery(id);
                var transacao = await _getTransacaoByIdQueryHandler.Handle(query);

                if (transacao == null)
                {
                    return NotFound();
                }
                return Ok(transacao);
            }
            catch (Exception ex)
            {
                // Logar o erro completo
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao consultar a transação.", error = ex.Message });
            }
        }

        /// <summary>
        /// Realiza um depósito em uma conta.
        /// </summary>
        /// <param name="dto">Dados do depósito.</param>
        /// <returns>A transação de depósito realizada.</returns>
        [HttpPost("deposito")]
        [ProducesResponseType(typeof(TransacaoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RealizarDeposito([FromBody] RealizarDepositoDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new RealizarDepositoCommand(dto);
                var depositoRealizado = await _realizarDepositoHandler.Handle(command);
                return CreatedAtAction(nameof(GetTransacaoById), new { id = depositoRealizado.Id }, depositoRealizado);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não encontrada"))
                    return NotFound(new { message = ex.Message });

                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao realizar o depósito.", error = ex.Message });
            }
        }
    }
}