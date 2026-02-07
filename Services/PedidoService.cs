using System.Linq;
using System.Globalization;
using System.Data;
using ProjetoTechStore_Volvo_2026.DTOs.Pedidos;
using ProjetoTechStore_Volvo_2026.Models;
using ProjetoTechStore_Volvo_2026.DTOs;
using ProjetoTechStore_Volvo_2026.Enums;
using ProjetoTechStore_Volvo_2026.Data;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace ProjetoTechStore_Volvo_2026.Service;

public class PedidoService
{
    public TechStoreContext _context;

    public PedidoService(TechStoreContext contexto)
    {
        _context = contexto;
    }

    public Pedido CriarPedido(PedidoEntradaDTO pedido)
    {
        var transacao = _context.Database.BeginTransaction();

        try
        {
            
            Pedido ped = new Pedido();
            ped.DataPedido = DateTime.Now;

            ped.NomeCliente = pedido.NomeCliente;
            ped.Id = pedido.Id;
            ped.Status = StatusPedido.PROCESSANDO;
            ped.Itens = new List<ItemPedido>();

            foreach(var produtoAux in pedido.Itens)
            {
                var produto = _context.Produtos.Find(produtoAux.ProdutoId);
                if(produto == null)
                {
                    throw new Exception($"O produto nome:{produtoAux.NomeProduto} - id:{produtoAux.ProdutoId} não foi encontrado.");
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
            _context.Pedidos.Add(ped);
            _context.SaveChanges();
            transacao.Commit();
            return ped;
        }
        catch
        {
            transacao.Rollback();
            throw new Exception($"Erro ao criar o pedido.");
        }
    }

    public List<PedidoRespostaDTO> ListarPedidos()
    {
        return _context.Pedidos
        .AsNoTracking()
        .OrderByDescending(p => p.DataPedido)
        .Select(p => new PedidoRespostaDTO
        {
            Id = p.Id,
            DataPedido = p.DataPedido,
            NomeCliente = p.NomeCliente,
            ValorTotal = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade),
            stt = p.Status,
            Itens = p.Itens.Select(item => new ItemPedidoRespostaDTO
            {
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                NomeProduto = item.Produto.Nome
            }).ToList()
        }).ToList();
    }

    public PedidoRespostaDTO? ProcurarPedidoPorID(int pedidoId)
    {
        return _context.Pedidos
        .AsNoTracking()
        .Where(p => p.Id == pedidoId)
        .Select(p => new PedidoRespostaDTO
        {
            Id = p.Id,
            DataPedido = p.DataPedido,
            NomeCliente = p.NomeCliente,
            stt = p.Status,
            ValorTotal = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade),
            Itens = p.Itens.Select(item => new ItemPedidoRespostaDTO
            {
                NomeProduto = item.Produto.Nome,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario

            }).ToList()
        }).FirstOrDefault();

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

    //Em um negócio não faz sentido deletar os pedidos, mesmo que sejam pedidos fraudulentos ou pedidos errados, mas está aqui por método de debug. 
    public bool DeletarPedidoPorID(int pedidoId)
    {
        var pedido = _context.Pedidos
        .Include(p => p.Itens)
        .FirstOrDefault(p => p.Id == pedidoId);

        if(pedido == null) { throw new Exception($"O Pedido {pedidoId} não foi encontrado.");}

        foreach(var Item in pedido.Itens)
        {
            var produto = _context.Produtos.Find(Item.ProdutoId);
            if(produto != null)
            {
                produto.Estoque+= Item.Quantidade;
            }
        }
        _context.Pedidos.Remove(pedido);
        _context.SaveChanges();
        return true;
    }

}