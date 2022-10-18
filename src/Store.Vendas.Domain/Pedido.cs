using Store.Core.DomainObjects;

namespace Store.Vendas.Domain
{
    public class Pedido
    {
        public static int MAXUNIDADESITEM => 15;
        public static int MINUNIDADESITEM => 1;

        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; private set; }
        public PedidoStatus PedidoStatus { get; set; }
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;
        private readonly List<PedidoItem> _pedidoItems;

        protected Pedido()
        {
            _pedidoItems = new List<PedidoItem>();
        }

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            //Green => fazer funcionar da forma mais basica e simples
            //ValorTotal = 200;

            //Refacotor
            ValidarQuantidadePermitida(pedidoItem);

            if (PedidoItemExistente(pedidoItem))
            {
                var itemExistente = _pedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);

                if (itemExistente is null) return;

                itemExistente.AdicionarUnidade(pedidoItem.Quantidade);

                pedidoItem = itemExistente;

                _pedidoItems.Remove(itemExistente);
            }

            _pedidoItems.Add(pedidoItem);
            CalcularValorPedido();
        }

        private bool PedidoItemExistente(PedidoItem pedidoItem)
        {
            return _pedidoItems.Any(p => p.ProdutoId == pedidoItem.ProdutoId);
        }

        private void ValidarQuantidadePermitida(PedidoItem pedidoItem)
        {
            var quantidadeItems = pedidoItem.Quantidade;

            if (PedidoItemExistente(pedidoItem))
            {
                var itemExistente = _pedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);

                if (itemExistente is not null)
                    quantidadeItems += itemExistente.Quantidade;
            }
            if (quantidadeItems > MAXUNIDADESITEM)
                throw new DomainException($"Máximo de {MAXUNIDADESITEM} unidades por produto");

        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(item => item.CalcularValor());
        }

        private void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido()
                {
                    ClienteId = clienteId,
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }
    }
}