using System.ComponentModel.DataAnnotations;

namespace ProjetoTechStore_Volvo_2026.DTOs.Produtos
{
    public class ProdutoEntradaDTO
    {
        public required string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public int CategoriaId { get; set; }
    }
}
