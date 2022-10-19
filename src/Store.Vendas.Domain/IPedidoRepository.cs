﻿using Store.Core.Data;

namespace Store.Vendas.Domain
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        void Adicionar(Pedido pedido);
        void Atualizar(Pedido pedido);
        Task<Pedido> ObterPedidoRascunhoPorClienteId(Guid clienteId);

        void AdicionarItem(PedidoItem pedidoItem);
        void AtualizarItem(PedidoItem item);
    }
}