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

        [HttpGet]
        public async Task <ActionResult<List<PedidoRespostaDTO>>> ListarPedidos()
        {
            try
            {
                var pedidos = await _Service.ListarPedidos();
                return Ok(pedidos);
            }
            catch
            {
                return StatusCode(500, new {message = "Erro ao listar pedidos."});
            }
        }

        [HttpGet("{pedidoId}")]
        public async Task <ActionResult<PedidoRespostaDTO>>? ProcurarPedidoPorID(int pedidoId)
        {
            var pedido = await _Service.ProcurarPedidoPorID(pedidoId);
            if(pedido == null)
            {
                return NotFound();
            }
            return Ok(pedido);
        }

        [HttpPost]
        public async Task<ActionResult<PedidoRespostaDTO>> CriarPedido([FromBody] PedidoEntradaDTO pedido)
        {
            try
            {
                var CriarPedido = await _Service.CriarPedido(pedido);
                var PedidoDTO = _Service.ConverterParaDTO(CriarPedido); //método de conversão para melhor retorno.

                return CreatedAtAction(nameof(ProcurarPedidoPorID), new {pedidoId = CriarPedido.Id}, PedidoDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPut("{pedidoId}")]
        public async Task<ActionResult<PedidoRespostaDTO>> UpdateStatusPedido(int pedidoId, StatusPedido _status)
        {
            try
            {
                await _Service.UpdateStatusPedido(pedidoId, _status);
                return Ok($"O pedido {pedidoId} teve seu status alterado para {_status}.");
            }
            catch(Exception ex)
            {
                if(ex.Message.Contains("Não foi possivel encontrar"))
                {
                    return NotFound(ex.Message);
                    
                }
                throw;
            }
        }

        [HttpDelete("{pedidoId}")]
        public async Task<IActionResult> DeletarPedidoPorID(int pedidoId)
        {
            var DeleteCheck = await _Service.DeletarPedidoPorID(pedidoId);
            if(DeleteCheck == false)
            {
                return NotFound();
            }

            return Ok($"O pedido {pedidoId} foi deletado.");
        }
    }
}