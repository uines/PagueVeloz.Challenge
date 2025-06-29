using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Documento { get; private set; } 
        public TipoCliente Tipo { get; private set; }

        public Guid ContaId { get; private set; }
        public Conta Conta { get; private set; }

        private Cliente() { }

        public Cliente(string nome, string documento, TipoCliente tipo)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Documento = documento;
            Tipo = tipo;


            Conta = new Conta(TipoConta.Corrente); 
            ContaId = Conta.Id;
        }
        public void AtualizarInformacoes(string novoNome, string novoDocumento)
        {
            if (string.IsNullOrWhiteSpace(novoNome)) throw new ArgumentException("Nome não pode ser vazio.", nameof(novoNome));
            if (string.IsNullOrWhiteSpace(novoDocumento)) throw new ArgumentException("Documento não pode ser vazio.", nameof(novoDocumento));

            Nome = novoNome;
            Documento = novoDocumento;
        }
        public void AlterarTipoConta(TipoConta novoTipo)
        {
            if (Conta == null)
                throw new InvalidOperationException("Cliente não possui uma conta associada.");
        }
    }
}