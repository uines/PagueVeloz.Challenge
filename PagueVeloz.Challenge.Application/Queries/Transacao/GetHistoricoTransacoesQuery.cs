namespace PagueVeloz.Challenge.Application.Queries.Transacao
{
    public class GetHistoricoTransacoesQuery
    {
        public Guid ContaId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20; 

        public GetHistoricoTransacoesQuery(Guid contaId)
        {
            ContaId = contaId;
        }
    }
}