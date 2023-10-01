using LanchesMac.Context;
using LanchesMac.Migrations;
using LanchesMac.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanchesMac.Areas.Admin.Servicos
{
    public class RelatorioVendasService
    {
        private readonly AppDbContext context;
        public RelatorioVendasService(AppDbContext _context)
        {
            context = _context;
        }

        //é usado para buscar pedidos com base em dados mínimos(minDate) e máximos(maxDate) especificados como restrições adicionais
        public async Task<List<Pedido>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            //Inicia uma consulta LINQ a partir da tabela de pedidos(context.Pedidos) e armazena o resultado em resultado.
            var resultado = from obj in context.Pedidos select obj;

            if (minDate.HasValue)
            {
                resultado = resultado.Where(x => x.PedidoEnviado >= minDate.Value);
            }

            if (maxDate.HasValue)
            {
                resultado = resultado.Where(x => x.PedidoEnviado <= maxDate.Value);
            }

         //A consulta inclui os detalhes dos pedidos(PedidoItens) e dos lanches associados(Lanche) por meio de Includee ThenInclude.
         //Os resultados são ordenados por dados de envio(PedidoEnviado) em ordem decrescente(do mais recente para o mais antigo) usando OrderByDescending.
            return await resultado
                .Include(l => l.PedidoItens)
                .ThenInclude(l => l.Lanche)
                .OrderByDescending(x => x.PedidoEnviado)
                .ToListAsync();
        }
    }
}
