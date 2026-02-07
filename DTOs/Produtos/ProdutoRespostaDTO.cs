namespace ProjetoTechStore_Volvo_2026.DTOs.Produtos
{
    public class ProdutoRespostaDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }

        // Só pra mostrar o nome ao invés do ID (pra facilitar pro usuário)
        public required string NomeCategoria { get; set; }
    }
}
