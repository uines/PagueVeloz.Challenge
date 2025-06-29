namespace PagueVeloz.Challenge.Application.Queries.Conta
{
    public class GetSaldoContaQuery
    {
        public Guid ContaId { get; set; }

        public GetSaldoContaQuery(Guid contaId)
        {
            ContaId = contaId;
        }
    }
}