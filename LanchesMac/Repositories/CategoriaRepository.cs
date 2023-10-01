using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using System.Collections.Generic;

namespace LanchesMac.Repositories
{
    //define uma classe CategoriaRepositoryque implementa uma interface ICategoriaRepository
    public class CategoriaRepository : ICategoriaRepository
    {
        //usada para acessar o contexto do banco de dados
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext contexto)
        {
            _context = contexto;
        }
        //consultou o contexto do banco de dados para obter todas as categorias disponíveis
        public IEnumerable<Categoria> Categorias => _context.Categorias;
    }
}
