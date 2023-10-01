using LanchesMac.Areas.Admin.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LanchesMac.Areas.Admin.Controllers
{
    //especifica que o controlador pertence à área "Admin". 
    [Area("Admin")]
    public class AdminRelatorioVendasController : Controller
    {
        private readonly RelatorioVendasService relatorioVendasService;

        public AdminRelatorioVendasController(RelatorioVendasService _relatorioVendasService)
        {
            relatorioVendasService = _relatorioVendasService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //é responsável por gerar e exibir um relatório de vendas simples com base nos dados especificados.
        public async Task<IActionResult> RelatorioVendasSimples(DateTime? minDate, DateTime? maxDate 
                                           /*string filter, int pageindex = 1, string sort = "Nome"*/)
        {
       //Aceita 2 parâmetros alternativos, minDatee maxDate, que determinam o intervalo de dados para o relatório.
       //Se os dados não foram fornecidos, são definidos desde o início do ano até os dados atuais.
            if (!minDate.HasValue)
            {
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = DateTime.Now;
            }

            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");

            //buscar os dados do relatório com base nos dados especificados e armazena o resultado em result
            var result = await relatorioVendasService.FindByDateAsync(minDate, maxDate);

            return View(result);
        }
    }
}
