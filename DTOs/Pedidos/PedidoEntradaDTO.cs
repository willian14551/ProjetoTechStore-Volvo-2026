using ProjetoTechStore_Volvo_2026.Enums;

namespace ProjetoTechStore_Volvo_2026.DTOs.Pedidos
{
    public class PedidoEntradaDTO
    {
        public string NomeCliente { get; set; } = String.Empty;

        public required MetodoPagamento _MetodoDePagamento {get;set;}

        public required List<ItemPedidoEntradaDTO> Itens { get; set; } = new();
    }
}
