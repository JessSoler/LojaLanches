using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanchesMac.Repositories
{
    //define uma classe PedidoRepositoryque implementa uma interface IPedidoRepository
    public class PedidoRepository : IPedidoRepository
    {
        //variável é usada para acessar o contexto do banco de dados para realizar operações relacionadas a pedidos
        private readonly AppDbContext _appDbContext;
        //variável é usada para acessar o carrinho de compras
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoRepository(AppDbContext appDbContext, CarrinhoCompra carrinhoCompra)
        {
            _appDbContext = appDbContext;
            _carrinhoCompra = carrinhoCompra;
        }

        public Pedido GetPedidoById(int pedidoId)
        {
            var pedido = _appDbContext.Pedidos.Include(pd => pd.PedidoItens
                         .Where(pd => pd.PedidoId == pedidoId))
                         .FirstOrDefault(p => p.PedidoId == pedidoId);

            return pedido;
        }
        //lista os pedidos
        public List<Pedido> GetPedidos()
        {
            return _appDbContext.Pedidos.ToList();
        }
        //cria os pedidos
        public void CriarPedido(Pedido pedido)
        {
            pedido.PedidoEnviado = DateTime.Now;
            //pedido.PedidoEntregueEm = DateTime.Now;

            _appDbContext.Pedidos.Add(pedido);
            _appDbContext.SaveChanges();

            var carrinhoCompraItens = _carrinhoCompra.CarrinhoCompraItens;

            foreach (var carrinhoItem in carrinhoCompraItens)
            {
                var pedidoDetail = new PedidoDetalhe()
                {
                    Quantidade = carrinhoItem.Quantidade,
                    LancheId = carrinhoItem.Lanche.LancheId,
                    PedidoId = pedido.PedidoId,
                    Preco = carrinhoItem.Lanche.Preco
                };
                _appDbContext.PedidoDetalhes.Add(pedidoDetail);
            }
            _appDbContext.SaveChanges();
        }
    }
}
