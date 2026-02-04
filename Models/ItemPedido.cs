using System.Text.Json.Serialization;

namespace ProjetoTechStore_Volvo_2026.Models
{
    public class ItemPedido
    {
        public int Id { get; set; }

        // FK para Pedido
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        // FK para Produto
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }

        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
