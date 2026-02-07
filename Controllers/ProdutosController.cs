using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.DTOs.Produtos;
using ProjetoTechStore_Volvo_2026.Models;

namespace ProjetoTechStore_Volvo_2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly TechStoreContext _context;

        public ProdutosController(TechStoreContext context)
        {
            _context = context;
        }

        // Get com filtros e paginação
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string nome,
            // O '?' pra garantir que a variável fique null caso não digite nada
            // Ou seja, não é obrigatório seu preenchimento
            [FromQuery] decimal? precoMin,
            [FromQuery] decimal? precoMax,
            [FromQuery] int pular = 0,
            [FromQuery] int pegar = 10)
        {
            // Inicio da Query
            var query = _context.Produtos
                .Include(p => p.Categoria) // Traz a categoria pra mostrar o nome
                .AsQueryable();

            // Aplicação de filtros apenas SE o usuário digitou algo
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.Nome.Contains(nome));
            }
            if (precoMin.HasValue)
            {
                query = query.Where(p => p.Preco >= precoMin.Value);
            }
            if (precoMax.HasValue)
            {
                query = query.Where(p => p.Preco >= precoMax.Value);
            }

            // Aplica a paginação e executa a busca (.ToList)
            var produtos = await query
                .Skip(pular)
                .Take(pegar)
                .ToListAsync();

            var listaDTO = produtos.Select(p => new ProdutoRespostaDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco,
                Estoque = p.Estoque,
                NomeCategoria = p.Categoria.Nome
            });

            return Ok(listaDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProdutoEntradaDTO dto)
        {
            // Verificar se a categoria existe primeiramente
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExiste)
            {
                return BadRequest("Categoria não encontrada. Verifique se digitou o nome corretamente.");
            }

            // Mapear DTO -> Entity
            var novoProduto = new Produto
            {
                Nome = dto.Nome,
                Preco = dto.Preco,
                Estoque = dto.Estoque,
                CategoriaId = dto.CategoriaId
            };

            _context.Produtos.Add(novoProduto);
            await _context.SaveChangesAsync();

            // Mapear para o DTO de resposta (pra poder devolver com o nome da categoria)
            // Já que o objeto Categoria ainda não está na memória, devolvemos o ID da Categoria ou busca pelo nome dela
            return CreatedAtAction(nameof(Get), new { id = novoProduto.Id }, dto);
        }

        // Buscar especificamente por ID (pro CreatedAtAction funcionar)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null) return NotFound();

            var dto = new ProdutoRespostaDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Estoque = produto.Estoque,
                NomeCategoria = produto.Categoria.Nome
            };

            return Ok(dto);
        }
    }
}