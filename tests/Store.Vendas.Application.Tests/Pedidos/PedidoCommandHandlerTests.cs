using Moq;
using Moq.AutoMock;
using Store.Vendas.Application.Commands;
using Store.Vendas.Domain;
using Xunit;

namespace Store.Vendas.Application.Tests.Pedidos
{
    public class PedidoCommandHandlerTests
    {
        [Fact(DisplayName = "Adicionar Item Novo Pedido com Sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoPedido_DeveExecutarComSucesso()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(Guid.NewGuid(),
                Guid.NewGuid(), "Produto Teste", 2, 100);

            var mocker = new AutoMocker();
            var pedidoHandler = mocker.CreateInstance<PedidoCommandHandler>();

            mocker.GetMock<IPedidoRepository>().Setup(lbda => lbda.UnitOfWork.Commit())
                .Returns(Task.FromResult(true));

            // Act
            var result = await pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            Assert.True(result);
            mocker.GetMock<IPedidoRepository>().Verify(lbda => lbda.Adicionar(It.IsAny<Pedido>()), Times.Once);
            mocker.GetMock<IPedidoRepository>().Verify(lbda => lbda.UnitOfWork.Commit(), Times.Once);
            //mocker.GetMock<IMediator>().Verify(lbda => lbda.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }
    }
}