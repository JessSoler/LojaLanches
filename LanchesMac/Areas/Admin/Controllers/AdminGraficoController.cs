using LanchesMac.Areas.Admin.Servicos;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminGraficoController : Controller
    {
        private readonly GraficoVendasService _graficoVendas;

        public AdminGraficoController(GraficoVendasService graficoVendas)
        {
            _graficoVendas = graficoVendas ?? throw
                new ArgumentNullException(nameof(graficoVendas));
        }

        //recebe um parâmetro dias, que é usado para determinar o período de dias para quem deseja obter dados de vendas de lanches.
        public JsonResult VendasLanches(int dias)
        {
            //_graficoVendas.GetVendasLanches(dias)para obter os dados de vendas de lanches para o período especificado.
            var lanchesVendasTotais = _graficoVendas.GetVendasLanches(dias);
            return Json(lanchesVendasTotais);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasMensal()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasSemanal()
        {
            return View();
        }
    }
}
