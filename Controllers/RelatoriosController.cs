using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.DTOs.Relatórios;

namespace ProjetoTechStore_Volvo_2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : Controller
    {
        private readonly TechStoreContext _context;

        public RelatoriosController(TechStoreContext context)
        {
            _context = context;
        }

        [HttpGet("vendas-por-categoria")]
        public async Task<IActionResult> GetVendasPorCategoriaGeral()
        {
            var relatorio = await _context.ItensPedidos
                .Include(i => i.Produto)
                .ThenInclude(p => p.Categoria)
                .GroupBy(i => i.Produto.Categoria.Nome)
                .Select(grupo => new VendasPorCategoriaDTO
                {
                    Categoria = grupo.Key,
                    ValorTotalVendido = grupo.Sum(i => i.PrecoUnitario * i.Quantidade)
                })
                .ToListAsync();
            return Ok(relatorio);
        }

        [HttpGet("vendas-por-categoria/{id}")]
        public async Task<IActionResult> GetVendasPorCategoriaID(int id)
        {
            var categoriaNome = await _context.Categorias
                .Where(c => c.Id == id)
                .Select(c => c.Nome)
                .FirstOrDefaultAsync();

            if (categoriaNome == null)
            {
                return NotFound("Categoria não encontrada.");
            }

            var totalVendido = await _context.ItensPedidos
                .Include(i => i.Produto)
                .Where(i => i.Produto.CategoriaId == id)
                .SumAsync(i => i.PrecoUnitario * i.Quantidade);

            var resultado = new VendasPorCategoriaDTO
            {
                Categoria = categoriaNome,
                ValorTotalVendido = totalVendido
            };

            return Ok(resultado);
        }
    }
}
