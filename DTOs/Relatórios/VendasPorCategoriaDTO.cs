namespace ProjetoTechStore_Volvo_2026.DTOs.Relatórios
{
    public class VendasPorCategoriaDTO
    {
        public int CategoriaId;
        public required string Categoria { get; set; }
        public decimal ValorTotalVendido { get; set; }
    }
}
