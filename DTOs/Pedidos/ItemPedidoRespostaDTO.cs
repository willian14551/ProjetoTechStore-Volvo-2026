namespace ProjetoTechStore_Volvo_2026.DTOs.Pedidos
{
    public class ItemPedidoRespostaDTO
    {
        public string NomeProduto {get;set;} = null!;
        public int Quantidade {get;set;}
        public decimal PrecoUnitario {get;set;}
    }
}
