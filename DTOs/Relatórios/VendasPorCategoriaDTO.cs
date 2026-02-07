using ProjetoTechStore_Volvo_2026.Models;

namespace ProjetoTechStore_Volvo_2026.DTOs.Relatórios
{
    public class VendasPorCategoriaDTO
    {
        public int CategoriaId;
        public required Categoria Categoria;

        public decimal ValorTotal; //Calculado na hora da criação
        //public decimal ValorMedio;

    }
}
