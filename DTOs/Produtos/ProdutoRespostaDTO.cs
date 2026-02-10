namespace ProjetoTechStore_Volvo_2026.DTOs.Produtos
{
    public class ProdutoRespostaDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public required string NomeCategoria { get; set; }
    }
}
