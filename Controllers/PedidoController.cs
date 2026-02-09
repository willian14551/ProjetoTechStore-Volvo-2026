using Microsoft.AspNetCore.Mvc;
using ProjetoTechStore_Volvo_2026.Models;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.DTOs.Pedidos;
using ProjetoTechStore_Volvo_2026.Enums;
using ProjetoTechStore_Volvo_2026.Service;


namespace ProjetoTechStore_Volvo_2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   public class PedidoController : ControllerBase
    {
        public PedidoService _Service;

        public PedidoController(PedidoService _svc)
        {
            _Service = _svc;
        }

        //Metodo listar pedidos lista todos os pedidos em formato de DTO:
        /*
            é retornado um edidorespostadto que contem: id, nomecliente, data do pedido, valor total do pedido e seu status.
            Além disso é retornado todos os items associados ao pedido com ujma classe item pedido que contem:
            nome do produto, quantidade e valor unitario.
        */
        [HttpGet]
        public ActionResult<List<PedidoRespostaDTO>> ListarPedidos()
        {
            try
            {
                var pedidos = _Service.ListarPedidos();
                return Ok(pedidos);
            }
            catch
            {
                return StatusCode(500, new {message = "Erro ao listar pedidos."});
            }
        }
        //Mesmo retorno do listarpedidos, mas apenas lista um pedido procurado por id.
        [HttpGet("{pedidoId}")]
        public ActionResult<PedidoRespostaDTO>? ProcurarPedidoPorID(int pedidoId)
        {
            var pedido = _Service.ProcurarPedidoPorID(pedidoId);
            if(pedido == null)
            {
                return NotFound();
            }
            return Ok(pedido);
        }

        //Permite a criação de pedidos e da adição de produtos.
        [HttpPost]
        public async Task<ActionResult<PedidoRespostaDTO>> CriarPedido([FromBody] PedidoEntradaDTO pedido)
        {
            try
            {
                var CriarPedido = await _Service.CriarPedido(pedido);
                var PedidoDTO = _Service.ConverterParaDTO(CriarPedido);

                return this.CreatedAtAction(nameof(ProcurarPedidoPorID), new {pedidoId = CriarPedido.Id}, PedidoDTO);
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { erro = ex.Message });
            }
        }
        //atualiza o status do pedido.
        [HttpPut("{pedidoId}")]
        public async Task<ActionResult<PedidoRespostaDTO>> UpdateStatusPedido(int pedidoId, StatusPedido _status)
        {
            var AtualizarPedido = await _Service.UpdateStatusPedido(pedidoId, _status);
            if(AtualizarPedido == false)
            {
                return NotFound();
            }

            return Ok();
        }
        //Deleta o pedido, apenas aqui por questões de debug pois não acho correto a deleção de pedidos.
        [HttpDelete("{pedidoId}")]
        public async Task<IActionResult> DeletarPedidoPorID(int pedidoId)
        {
            var DeleteCheck = await _Service.DeletarPedidoPorID(pedidoId);
            if(DeleteCheck == false)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}