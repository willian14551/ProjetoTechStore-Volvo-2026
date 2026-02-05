using System.Linq;
using System.Globalization;
using System.Data;
using ProjetoTechStore_Volvo_2026.Models;
using ProjetoTechStore_Volvo_2026.DTOs;
using ProjetoTechStore_Volvo_2026.Enums;
using ProjetoTechStore_Volvo_2026.Data;
using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.DTOs.Pedidos;

namespace Techstore.Service;

public class PedidoService
{
    public TechStoreContext _context;

    public PedidoService(TechStoreContext contexto)
    {
        _context = contexto;
    }

    public Pedido CriarPedido(PedidoCriarDTO pedido)
    {
        Pedido ped = new Pedido();
        ped.DataPedido = DateTime.Now;

        ped.NomeCliente = pedido.NomeCliente;
        ped.Id = pedido.Id;
        ped.Itens = new List<ItemPedido>();

        foreach(var produtoAux in pedido.Itens)
        {
            var produto = _context.Produtos.Find(produtoAux.ProdutoId);
            if(produto == null)
            {
                throw new Exception($"O produto {produtoAux.NomeProduto} - {produtoAux.ProdutoId} não foi encontrado.");
            }

            if (produto.Estoque < produtoAux.Quantidade)
            {
                throw new Exception($"{produto.Nome} não possui estoque suficiente para realizar a transação.");
            }

            produto.Estoque-=produtoAux.Quantidade;

            var NovoItemPedido = new ItemPedido
            {
                Pedido = ped,
                Produto = produto,
                ProdutoId = produto.Id,
                PedidoId = ped.Id,
                Quantidade = produtoAux.Quantidade,
                PrecoUnitario = produtoAux.PrecoUnitario
            };
            ped.Itens.Add(NovoItemPedido);
        }
        ped.Status = StatusPedido.PROCESSANDO;
        return ped;
    }

    //substituir por DTO mais a frente:
    public List<PedidoRespostaDTO> ListarPedidos()
    {
        return _context.Pedidos
        .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
        .OrderByDescending(p => p.DataPedido).Select(p => new PedidoRespostaDTO
        {
            Id = p.Id,
            DataPedido = p.DataPedido,
            NomeCliente = p.NomeCliente,
            ValorTotal = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade),
            stt = p.Status,
            Itens = p.Itens.Select(item => new ItemPedidoRespostaDTO
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                NomeProduto = item.Produto.Nome
            }).ToList()
        }).ToList();
    }

    public Pedido? ProcurarPedidoPorID(int pedidoId)
    {
        return _context.Pedidos.Find(pedidoId);
    }

    public bool UpdateStatusPedido(int pedidoId, StatusPedido stt)
    {
        var pedido = _context.Pedidos.Find(pedidoId);
        if(pedido == null)
        {
            throw new Exception($"Não foi possivel encontrar o pedido {pedidoId}.");
        }
        if(stt == pedido.Status)
        {
            throw new Exception($"Não foi possivel alterar o status do pedido {pedidoId}.");
        }
        pedido.Status = stt;
        return true;
    }
}