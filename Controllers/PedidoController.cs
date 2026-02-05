using Microsoft.AspNetCore.Mvc;
using ProjetoTechStore_Volvo_2026.Models;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.DTOs.Pedidos;
using ProjetoTechStore_Volvo_2026.Enums;
using ProjetoTechStore_Volvo_2026.Service;


namespace ProjetoTechStore_Volvo_2026.Controllers
{
    public class PedidoController : ControllerBase
    {
        public PedidoService _Service;

        public PedidoController(PedidoService _svc)
        {
            _Service = _svc;
        }


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

        [HttpGet("{pedidoIdd}")]
        public ActionResult<PedidoRespostaDTO>? ProcurarPedidoPorID(int pedidoId)
        {
            var pedido = _Service.ProcurarPedidoPorID(pedidoId);
            if(pedido == null)
            {
                return NotFound();
            }
            return Ok(pedido);
        }

        [HttpPost]
        public ActionResult<PedidoCriarDTO> CriarPedido([FromBody] PedidoCriarDTO pedido)
        {
            var CriarPedido = _Service.CriarPedido(pedido);
            if(CriarPedido == null)
            {
                return BadRequest();
            }
            return CreatedAtAction($"Pedido com o id: ", new {id = pedido.Id}, pedido);
        }

        [HttpPut("{pedidoId}")]
        public ActionResult<PedidoRespostaDTO> UpdateStatusPedido(int pedidoId, StatusPedido _status)
        {
            var AtualizarPedido = _Service.UpdateStatusPedido(pedidoId, _status);
            if(AtualizarPedido == false)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("pedidoId")]
        public IActionResult DeletarPedidoPorID(int pedidoId)
        {
            var DeleteCheck = _Service.DeletarPedidoPorID(pedidoId);
            if(DeleteCheck == false)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}