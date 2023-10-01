using LanchesMac.Context;
using LanchesMac.Migrations;
using LanchesMac.Models;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LanchesMac.Areas.Admin.Controllers
{
    //especifica que o controlador pertence à área "Admin". 
    [Area("Admin")]
    //indica que apenas usuários com a função "Admin" têm permissão para acessar
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        //é o contexto do banco de dados usado para acessar os dados relacionados aos pedidos.
        private readonly AppDbContext _context;

        public AdminPedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminPedidos
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Pedidos.ToListAsync());
        //}

       //é usada para exibir os detalhes de um pedido específico com base em seu ID.
      //Consulta o banco de dados para recuperar o pedido, incluindo os itens do pedido e os lanches associados.
        public IActionResult PedidoLanches(int? id)
        {
            var pedido = _context.Pedidos
                          .Include(pd => pd.PedidoItens)
                          .ThenInclude(l => l.Lanche)
                          .FirstOrDefault(p => p.PedidoId == id);

           if (pedido == null)
           {
              Response.StatusCode = 404;
              return View("PedidoNotFound", id.Value);
           }

            PedidoLancheViewModel pedidoLanches = new PedidoLancheViewModel()
           {
                Pedido = pedido,
                PedidoDetalhes = pedido.PedidoItens
           };
           return View(pedidoLanches);
        }

        //usada para exibir uma lista paginada de pedidos
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "Nome")
        {
            //Consulte o banco de dados para obter os pedidos, aplicando filtros e ordenação conforme necessário.
            var resultado = _context.Pedidos.AsNoTracking()
                           .AsQueryable();


            if (!string.IsNullOrWhiteSpace(filter))
            {
                resultado = resultado.Where(p => p.Nome.Contains(filter));
            }

            var model = await PagingList.CreateAsync(resultado, 5, pageindex, sort, "Nome");

            model.RouteValue = new RouteValueDictionary { { "filter", filter } };

            return View(model);
        }

        //o CRUD  segue o mesmo raciocinio do lanches
        // GET: Admin/AdminPedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.PedidoId == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Admin/AdminPedidos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminPedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PedidoId,Nome,Sobrenome,Endereco1,Endereco2,Cep,Estado,Cidade,Telefone,Email,PedidoEnviado,PedidoEntregueEm")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        // GET: Admin/AdminPedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        // POST: Admin/AdminPedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PedidoId,Nome,Sobrenome,Endereco1,Endereco2,Cep,Estado,Cidade,Telefone,Email,PedidoEnviado,PedidoEntregueEm")] Pedido pedido)
        {
            if (id != pedido.PedidoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.PedidoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        // GET: Admin/AdminPedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.PedidoId == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Admin/AdminPedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //verifica se um pedido com o ID especificado não existe em banco de dados.
        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.PedidoId == id);
        }
    }
}
