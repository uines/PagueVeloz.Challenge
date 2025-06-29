namespace PagueVeloz.Challenge.Application.Queries.Transacao
{
    public class GetTransacaoByIdQuery
    {
        public Guid TransacaoId { get; set; }

        public GetTransacaoByIdQuery(Guid transacaoId)
        {
            TransacaoId = transacaoId;
        }
    }
}