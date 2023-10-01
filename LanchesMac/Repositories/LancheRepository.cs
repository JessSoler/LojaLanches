using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LanchesMac.Repositories
{
    //define uma classe LancheRepositoryque implementa uma interface ILancheRepository
    public class LancheRepository : ILancheRepository
    {
        //declara uma variável de chamada privada _contextdo tipo AppDbContext.Essa variável é
        //usada para acessar o contexto do banco de dados.
        private readonly AppDbContext _context;

        public LancheRepository(AppDbContext contexto)
        {
            _context = contexto;
        }

        //retorna uma coleção de lanches.Ela consulta o contexto do banco de dados para obter todos os lanches
        //e suas respectivas categorias, usando o método Include para carregar os dados relacionados.Os lanches
        //disponíveis estão para leitura através dessa propriedade.
        public IEnumerable<Lanche> Lanches => _context.Lanches.Include(c => c.Categoria);

        //retorna uma coleção de lanches preferidos.Ela consulta o contexto do banco de dados para obter os lanches
        //que estão marcados como preferidos (IsLanchePreferido) e também carrega as informações de categoria relacionadas
        //a esses lanches.
        public IEnumerable<Lanche> LanchesPreferidos => _context.Lanches.Where(p => p.IsLanchePreferido).Include(c => c.Categoria);

        //consulta o contexto do banco de dados para encontrar o lanche com o ID correspondente e retorna o primeiro
        //lanche encontrado ou nullse nenhum for encontrado.
        public Lanche GetLancheById(int lancheId) => _context.Lanches.FirstOrDefault(l => l.LancheId == lancheId);
    }
}
