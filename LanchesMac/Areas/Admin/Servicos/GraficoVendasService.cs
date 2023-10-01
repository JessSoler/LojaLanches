using LanchesMac.Context;
using LanchesMac.Migrations;
using LanchesMac.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LanchesMac.Areas.Admin.Servicos
{
    public class GraficoVendasService
    {
        private readonly AppDbContext context;

        public GraficoVendasService(AppDbContext context)
        {
            this.context = context;
        }

        //é usado para obter dados de vendas de lanches ao longo de um período de tempo especificado(por padrão, 360 dias).
        //Realiza uma consulta LINQ que junta informações de PedidoDetalhes(itens de pedidos) e Lanches(produtos) com base em IDs relacionados.
        //Faz um filtro para selecionar apenas os detalhes de pedidos cujos dados de envio(Pedido.PedidoEnviado) sejam maiores ou iguais aos dados de início do período.
        //Agrupa os resultados com base no ID do lanche e no nome do lanche, calculando a quantidade total vendida(LanchesQuantidade) e o valor total das vendas(LanchesValorTotal) para cada lanche.
        public List<LancheGrafico> GetVendasLanches(int dias = 360)
        {
            //Calcula o dado de início do período subtraindo o número de dias especificado do dado atual(DateTime.Now)
            var data = DateTime.Now.AddDays(-dias);

            var lanches = (from pd in context.PedidoDetalhes
                           join l in context.Lanches on pd.LancheId equals l.LancheId
                           where pd.Pedido.PedidoEnviado >= data
                           group pd by new { pd.LancheId, l.Nome }
                           into g
                           select new
                           {
                               LancheNome = g.Key.Nome,
                               LanchesQuantidade = g.Sum(q => q.Quantidade),
                               LanchesValorTotal = g.Sum(a => a.Preco * a.Quantidade)
                           });

            var lista = new List<LancheGrafico>();

            foreach(var item in lanches)
            {
                var lanche = new LancheGrafico();
                lanche.LancheNome = item.LancheNome;
                lanche.LanchesQuantidade = item.LanchesQuantidade;
                lanche.LanchesValorTotal = item.LanchesValorTotal;
                lista.Add(lanche);
            }
            return lista;
        }
    }
}
