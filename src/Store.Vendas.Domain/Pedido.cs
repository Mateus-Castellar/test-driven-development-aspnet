using FluentValidation.Results;
using Store.Core.DomainObjects;

namespace Store.Vendas.Domain
{
    public class Pedido : Entity, IAggregateRoot
    {
        public static int MAXUNIDADESITEM => 15;
        public static int MINUNIDADESITEM => 1;

        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; private set; }
        public decimal Desconto { get; set; }
        public PedidoStatus PedidoStatus { get; set; }
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;
        private readonly List<PedidoItem> _pedidoItems;
        public Voucher? Voucher { get; set; }
        public bool VoucherUtilizado { get; set; }

        protected Pedido()
        {
            _pedidoItems = new List<PedidoItem>();
        }

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            ValidarQuantidadePermitida(pedidoItem);

            if (PedidoItemExistente(pedidoItem))
            {
                var itemExistente = _pedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId)
                    ?? throw new ArgumentNullException(nameof(pedidoItem), "item nulo");

                itemExistente.AdicionarUnidade(pedidoItem.Quantidade);

                pedidoItem = itemExistente;

                _pedidoItems.Remove(itemExistente);
            }

            _pedidoItems.Add(pedidoItem);
            CalcularValorPedido();
        }

        public bool PedidoItemExistente(PedidoItem pedidoItem)
        {
            return _pedidoItems.Any(p => p.ProdutoId == pedidoItem.ProdutoId);
        }

        private void ValidarPedidoItemInexistente(PedidoItem pedidoItem)
        {
            if (PedidoItemExistente(pedidoItem) is false)
                throw new DomainException("O item não pertence ao pedido!");
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
            CalcularValorTotalDesconto();
        }

        private void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        public void AtualizarItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);
            ValidarQuantidadePermitida(pedidoItem);

            var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId)
                 ?? throw new ArgumentNullException(nameof(pedidoItem), "item nulo");

            _pedidoItems.Remove(itemExistente);
            _pedidoItems.Add(pedidoItem);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);

            _pedidoItems.Remove(pedidoItem);

            CalcularValorPedido();
        }

        public ValidationResult AplicarVoucher(Voucher voucher)
        {
            var result = voucher.ValidarSeAplicavel();

            if (result.IsValid is false) return result;

            Voucher = voucher;
            VoucherUtilizado = true;

            CalcularValorTotalDesconto();

            return result;
        }

        public void CalcularValorTotalDesconto()
        {
            if (VoucherUtilizado is false)
                return;

            decimal desconto = 0;
            var valor = ValorTotal;

            if (Voucher?.TipoDescontoVoucher == TipoDescontoVoucher.Valor)
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.PercentualDesconto.HasValue)
                {
                    desconto = (ValorTotal * Voucher.PercentualDesconto.Value) / 100;
                    valor -= desconto;
                }
            }

            ValorTotal = valor < 0 ? 0 : valor;
            Desconto = desconto;
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