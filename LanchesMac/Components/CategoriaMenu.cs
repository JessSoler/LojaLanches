using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

//este componente de visualização CategoriaMenué responsável por recuperar uma lista de categorias do repositório de
//categorias e passá-la para uma visão, onde pode ser usado para criar uma lista de links ou botões que permite aos usuários
//filtrar os lanches por categoria
namespace LanchesMac.Components
{
    public class CategoriaMenu : ViewComponent
    {
        private readonly ICategoriaRepository _categoriaRepository;
        public CategoriaMenu(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public IViewComponentResult Invoke()
        {
            var categorias = _categoriaRepository.Categorias.OrderBy(p => p.CategoriaNome);
            return View(categorias);
        }
    }
}