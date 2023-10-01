using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LanchesMac.Controllers
{
    public class LancheController : Controller
    {
        private ICategoriaRepository _categoriaRepository { get; }
        private readonly ILancheRepository _lancheRepository;

        public LancheController(ICategoriaRepository categoriaRepository,
            ILancheRepository lancheRepository)
        {
            _categoriaRepository = categoriaRepository;
            _lancheRepository = lancheRepository;
        }
        //é usado para listar os lanches com base na categoria fornecida
        public IActionResult List(string categoria)
        {
            //string _categoria = categoria;
            IEnumerable<Lanche> lanches;
            string categoriaAtual = string.Empty;

            if (string.IsNullOrEmpty(categoria))
            {
                lanches = _lancheRepository.Lanches.OrderBy(p => p.LancheId);
                categoriaAtual = "Todos os lanches";
            }
            else
            {
                lanches = _lancheRepository.Lanches
                           .Where(p => p.Categoria.CategoriaNome.Equals(categoria))
                           .OrderBy(p => p.Nome);

                categoriaAtual = categoria;
            }

            var lancheListViewModel = new LancheListViewModel
            {
                Lanches = lanches,
                CategoriaAtual = categoriaAtual
            };

            return View(lancheListViewModel);
        }
        //exibe os detalhes de um lanche específico com base no lancheId,
        //ele consulta o ILancheRepositorypara obter os detalhes do lanche conforme
        public ViewResult Details(int lancheId)
        {
            var lanche = _lancheRepository.Lanches.FirstOrDefault(d => d.LancheId == lancheId);
            if (lanche == null)
            {
                return View("~/Views/Error/Error.cshtml");
            }
            return View(lanche);
        }
        //usado para pesquisas com base em uma string de pesquisa(searchString),se a sequência de pesquisa estiver
        //em branco, todos os lanches serão selecionados. Caso contrário, ele consulta o ILancheRepository
        public ViewResult Search(string searchString)
        {
            string _searchString = searchString;
            IEnumerable<Lanche> lanches;
            string currentCategory = string.Empty;

            if (string.IsNullOrEmpty(_searchString))
            {
                lanches = _lancheRepository.Lanches.OrderBy(p => p.LancheId);
            }
            else
            {
                lanches = _lancheRepository.Lanches.Where(p => p.Nome.ToLower().Contains(_searchString.ToLower()));
            }

            return View("~/Views/Lanche/List.cshtml", new LancheListViewModel
            {
                Lanches = lanches,
                CategoriaAtual = "Todos os lanches"
            });
        }

    }
}