using ProjetoTechStore_Volvo_2026.Enums;

namespace ProjetoTechStore_Volvo_2026.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        // string.Empty garante que o NomeCliente não seja Null
        public List<ItemPedido> Itens { get; set; } = new();

        public StatusPedido Status;

    }
}
