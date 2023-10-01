using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;

namespace LanchesMac.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILancheRepository _lancheRepository;
        public HomeController(ILancheRepository lancheRepository)
        {
            _lancheRepository = lancheRepository;
        }
        //Nessa parte do código, o método Index()cria uma instância do modelo HomeViewModel,
        //preenche sua propriedade LanchesPreferidoscom os dados obtidos do _lancheRepositorye,
        //em seguida, retorna uma visualização(uma página da web) chamada "Index", passando o homeViewModelcomo modelo.
        //Isso resulta na exibição de uma página da web com os dados dos lanches preferidos quando o método é acessado.
        public ViewResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                LanchesPreferidos = _lancheRepository.LanchesPreferidos
            };
            return View(homeViewModel);
        }

        public ViewResult AccessDenied()
        {
            return View();
        }
    }
}
