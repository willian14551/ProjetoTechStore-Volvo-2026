using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.DTOs.Categorias;
using ProjetoTechStore_Volvo_2026.Models;

namespace ProjetoTechStore_Volvo_2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly TechStoreContext _context;

        public CategoriasController(TechStoreContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CategoriaEntradaDTO dto)
        {
            var novaCategoria = new Categoria
            {
                Nome = dto.Nome
            };

            _context.Categorias.Add(novaCategoria);
            await _context.SaveChangesAsync();

            var dtoResposta = new CategoriaRespostaDTO
            {
                Id = novaCategoria.Id,
                Nome = novaCategoria.Nome
            };

            return CreatedAtAction(nameof(Get), new { id = novaCategoria.Id }, dtoResposta);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categorias = await _context.Categorias.ToListAsync();

            var listaDTO = categorias.Select(c => new CategoriaRespostaDTO
            {
                Id = c.Id,
                Nome = c.Nome
            });

            return Ok(listaDTO);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var temProdutos = await _context.Produtos.AnyAsync(p => p.CategoriaId == id);

            if (temProdutos)
            {
                return BadRequest("Não foi possível deletar esta categoria pois existe produtos que dependem dela.");
                }

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return Ok($"A categoria {categoria.Nome} do Id {categoria.Id} foi deletada.");
        }   
    }
}

