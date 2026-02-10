using System.Text.Json.Serialization; 

namespace ProjetoTechStore_Volvo_2026.Models
{
    public class ItemPedido
    {
        
        public int Id { get; set; }

        public int PedidoId { get; set; }
        [JsonIgnore]    
        public Pedido Pedido { get; set; } = null!;


        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
