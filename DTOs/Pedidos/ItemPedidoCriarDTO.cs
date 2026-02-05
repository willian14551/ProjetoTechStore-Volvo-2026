namespace ProjetoTechStore_Volvo_2026.DTOs.Pedidos
{
    public class ItemPedidoCriarDTO
    {
        public int ProdutoId {get;set;}
        public required string NomeProduto {get;set;} = string.Empty;
        public int Quantidade {get;set;}
        public decimal PrecoUnitario {get;set;}
    }
}
