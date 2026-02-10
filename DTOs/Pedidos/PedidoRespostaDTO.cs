using ProjetoTechStore_Volvo_2026.Enums;

namespace ProjetoTechStore_Volvo_2026.DTOs.Pedidos
{
    public class PedidoRespostaDTO
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public string NomeCliente { get; set; } = String.Empty;
        public decimal ValorTotal {get;set;} 
        public List<ItemPedidoRespostaDTO> Itens { get; set; } = new();
        public StatusPedido stt {get;set;} 
    }
}
