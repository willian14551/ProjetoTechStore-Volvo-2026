using ProjetoTechStore_Volvo_2026.Enums;

namespace ProjetoTechStore_Volvo_2026.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public required string NomeCliente { get; set; }
        public required List<ItemPedido> Itens { get; set; } = new();
        public StatusPedido Status;

    }
}
