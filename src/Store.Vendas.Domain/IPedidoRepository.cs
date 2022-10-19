using Store.Core.Data;

namespace Store.Vendas.Domain
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        void Adicionar(Pedido pedido);
    }
}