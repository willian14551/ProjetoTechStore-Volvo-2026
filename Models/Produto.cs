namespace ProjetoTechStore_Volvo_2026.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }

        // FK de CategoriaId
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
        // null! é um operador que avisa para o compilador que a variável não vai ficar null
    }
}
