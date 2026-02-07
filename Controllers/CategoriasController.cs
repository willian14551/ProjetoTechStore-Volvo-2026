using Microsoft.AspNetCore.Mvc;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.Models;

namespace ProjetoTechStore_Volvo_2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly TechStoreContext _context;

        public CategoriasController (TechStoreContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Categorias.ToList());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Verifica se tem algum produto usando essa categoria  
            var temProdutos = _context.Produtos.Any(p => p.CategoriaId == id);
            if (temProdutos)
            {
                return BadRequest("Não foi possível deletar está categoria " +
                    "pois existe produtos vinculados a ela.");
            }

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound("Categoria não encontrada.");
            }

            // remove e salva
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
