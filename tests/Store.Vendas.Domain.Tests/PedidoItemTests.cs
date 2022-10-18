using Store.Core.DomainObjects;
using Xunit;

namespace Store.Vendas.Domain.Tests
{
    public class PedidoItemTests
    {
        [Fact(DisplayName = "Novo Item Pedido com unidades Abaixo Do Permitido")]
        [Trait("Categoria", "Item Pedido Tests")]
        public void AdicionarItemPedido_ItemAbaixoDoPermitido_DeveRetornarException()
        {
            // Arrange & Act & Assert
            Assert.Throws<DomainException>(() => new PedidoItem(Guid.NewGuid(), "Produto Teste", Pedido.MINUNIDADESITEM - 1, 100));
        }

    }
}