using MediatR;
using Store.Vendas.Application.Events;
using Store.Vendas.Domain;

namespace Store.Vendas.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarItemPedidoCommand, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMediator _mediator;

        public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediator mediator)
        {
            _pedidoRepository = pedidoRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AdicionarItemPedidoCommand message, CancellationToken cancellationToken)
        {
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(message.ClienteId);
            var pedidoItem = new PedidoItem(message.ProdutoId, message.Nome, message.Quantidade, message.ValorUnitario);
            pedido.AdicionarItem(pedidoItem);

            _pedidoRepository.Adicionar(pedido);

            pedido.AdicionarEvento(new PedidoItemAdicionadoEvent(pedido.ClienteId, pedido.Id, message.ProdutoId,
                message.Nome, message.ValorUnitario, message.Quantidade));

            return await _pedidoRepository.UnitOfWork.Commit();
        }
    }
}
