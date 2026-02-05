namespace ProjetoTechStore_Volvo_2026.DTOs.Pedidos
{
    public class ItemPedidoRespostaDTO
    {
        public int ProdutoId {get;set;}
        public required string NomeProduto {get;set;} 
        public int Quantidade {get;set;}
        public decimal PrecoUnitario {get;set;}
    }
}
