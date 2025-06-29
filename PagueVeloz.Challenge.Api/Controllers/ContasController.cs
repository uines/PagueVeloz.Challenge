using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Queries.Conta;
using PagueVeloz.Challenge.Application.Queries.Transacao;
using PagueVeloz.Challenge.Application.Handlers;

namespace PagueVeloz.Challenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly GetSaldoContaQueryHandler _getSaldoContaQueryHandler;
        private readonly GetHistoricoTransacoesQueryHandler _getHistoricoTransacoesQueryHandler;

        public ContasController(
            GetSaldoContaQueryHandler getSaldoContaQueryHandler,
            GetHistoricoTransacoesQueryHandler getHistoricoTransacoesQueryHandler)
        {
            _getSaldoContaQueryHandler = getSaldoContaQueryHandler;
            _getHistoricoTransacoesQueryHandler = getHistoricoTransacoesQueryHandler;
        }

        /// <summary>
        /// Obtém o saldo detalhado de uma conta.
        /// </summary>
        /// <param name="id">ID da conta.</param>
        /// <returns>O saldo da conta.</returns>
        [HttpGet("{id}/saldo")]
        [ProducesResponseType(typeof(SaldoContaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSaldoConta(Guid id)
        {
            try
            {
                var query = new GetSaldoContaQuery(id);
                var saldo = await _getSaldoContaQueryHandler.Handle(query);
                return Ok(saldo);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Logar o erro completo
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao consultar o saldo.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o histórico de transações de uma conta.
        /// </summary>
        /// <param name="id">ID da conta.</param>
        /// <param name="dataInicio">Data de início para filtrar transações (opcional).</param>
        /// <param name="dataFim">Data de fim para filtrar transações (opcional).</param>
        /// <param name="skip">Número de registros a pular (para paginação).</param>
        /// <param name="take">Número de registros a retornar (para paginação).</param>
        /// <returns>Uma lista de transações.</returns>
        [HttpGet("{id}/historico")]
        [ProducesResponseType(typeof(IEnumerable<TransacaoResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHistoricoTransacoes(
            Guid id,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
            try
            {
                if (take > 100) take = 100;

                var query = new GetHistoricoTransacoesQuery(id)
                {
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    Skip = skip,
                    Take = take
                };
                var historico = await _getHistoricoTransacoesQueryHandler.Handle(query);
                return Ok(historico);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao consultar o histórico de transações.", error = ex.Message });
            }
        }
    }
}