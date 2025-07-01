namespace PagueVeloz.Challenge.Application.Queries.Cliente
{
    public class GetClienteByIdQuery
    {
        public Guid ClienteId { get; set; }

        public GetClienteByIdQuery(Guid clienteId)
        {
            ClienteId = clienteId;
        }
    }
}