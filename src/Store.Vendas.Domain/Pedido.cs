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

        public void AdicionarItem(PedidoItem item)
        {
            //Green => fazer funcionar da forma mais basica e simples
            //ValorTotal = 200;

            //Refacotor
            if (item.Quantidade > MAXUNIDADESITEM)
                throw new DomainException($"Máximo de {MAXUNIDADESITEM} unidades por produto");



            if (_pedidoItems.Any(p => p.ProdutoId == item.ProdutoId))
            {
                var itemExistente = _pedidoItems.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);

                if (itemExistente is null) return;

                itemExistente.AdicionarUnidade(item.Quantidade);
                item = itemExistente;

                _pedidoItems.Remove(itemExistente);
            }

            _pedidoItems.Add(item);
            CalcularValorPedido();
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