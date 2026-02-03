namespace ProjetoTechStore_Volvo_2026.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }

        // FK de CategoriaId
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}
