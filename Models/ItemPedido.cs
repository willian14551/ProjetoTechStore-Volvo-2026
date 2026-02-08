using System.Text.Json.Serialization; 

namespace ProjetoTechStore_Volvo_2026.Models
{
    public class ItemPedido
    {
        
        public int Id { get; set; }

        // FK para Pedido
        public int PedidoId { get; set; }
        [JsonIgnore]    
        public Pedido Pedido { get; set; } = null!;


        // FK para Produto
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;
        // null! é um operador que avisa para o compilador que a variável não vai ficar null

        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
