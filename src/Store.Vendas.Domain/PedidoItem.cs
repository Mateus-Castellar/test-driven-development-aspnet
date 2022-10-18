using Store.Core.DomainObjects;

namespace Store.Vendas.Domain
{
    public class PedidoItem
    {
        public Guid ProdutoId { get; private set; }
        public string ProdutoNome { get; private set; } = null!;
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }

        public PedidoItem(Guid produtoId, string produtoNome, int quantidade, decimal valorUnitario)
        {
            if (quantidade < Pedido.MINUNIDADESITEM)
                throw new DomainException($"Minímo de {Pedido.MAXUNIDADESITEM} unidades por produto");

            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        internal void AdicionarUnidade(int quantidades)
        {
            Quantidade += quantidades;
        }

        internal decimal CalcularValor()
        {
            return Quantidade * ValorUnitario;
        }
    }
}