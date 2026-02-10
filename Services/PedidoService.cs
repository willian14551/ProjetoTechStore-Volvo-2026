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
using Microsoft.IdentityModel.Tokens;

namespace ProjetoTechStore_Volvo_2026.Service;

public class PedidoService
{
    public TechStoreContext _context;

    public PedidoService(TechStoreContext contexto)
    {
        _context = contexto;
    }

    public async Task<Pedido> CriarPedido(PedidoEntradaDTO pedido)
    {
        var estrategiaExecucao = _context.Database.CreateExecutionStrategy();
        return await estrategiaExecucao.ExecuteAsync(async () =>
        {
            using var transacao = await _context.Database.BeginTransactionAsync();
            try
            {
                if(pedido.Itens.IsNullOrEmpty())
                {
                    throw new Exception($"Erro ao criar o pedido: Um pedido deve conter pelo menos um item.");
                }

                Pedido ped = new Pedido
                {
                    NomeCliente = pedido.NomeCliente,
                    DataPedido = DateTime.Now,
                    Status = StatusPedido.PROCESSANDO,
                    Itens = new List<ItemPedido>()
                };

                foreach(var produtoAux in pedido.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(produtoAux.ProdutoId);
                    if(produto == null)
                    {
                        throw new Exception($"O produto com o id:{produtoAux.ProdutoId} não foi encontrado.");
                    }
                    if (produto.Estoque < produtoAux.Quantidade)
                    {
                        throw new Exception($"{produto.Nome} não possui estoque suficiente para realizar a transação.");
                    }

                    produto.Estoque-=produtoAux.Quantidade;
                    var NovoItemPedido = new ItemPedido
                    {
                        ProdutoId = produto.Id,
                        Quantidade = produtoAux.Quantidade,
                        PrecoUnitario = produto.Preco,
                    };
                    ped.Itens.Add(NovoItemPedido);
                }
                _context.Pedidos.Add(ped);
                await _context.SaveChangesAsync();

                await transacao.CommitAsync();

                return ped;
            }
            catch(Exception ex)
            {
                await transacao.RollbackAsync();
                var mensagemErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"Erro ao criar o pedido: {mensagemErro}.");
            }
        });
    }

    public async Task<List<PedidoRespostaDTO>> ListarPedidos()
    {
        return await _context.Pedidos
        .AsNoTracking()
        .OrderByDescending(p => p.DataPedido)
        .Select(p => new PedidoRespostaDTO 
        {
            Id = p.Id,
            DataPedido = p.DataPedido,
            NomeCliente = p.NomeCliente,
            ValorTotal = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade),
            Itens = p.Itens.Select(item => new ItemPedidoRespostaDTO
            {
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                NomeProduto = item.Produto.Nome
            }).ToList()
        }).ToListAsync(); 
    }

    public async Task<PedidoRespostaDTO?> ProcurarPedidoPorID(int pedidoId)
    {
        return await _context.Pedidos
        .AsNoTracking()
        .Where(p => p.Id == pedidoId)
        .Select(p => new PedidoRespostaDTO
        {
            Id = p.Id,
            DataPedido = p.DataPedido,
            NomeCliente = p.NomeCliente,
            ValorTotal = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade),
            Itens = p.Itens.Select(item => new ItemPedidoRespostaDTO
            {
                NomeProduto = item.Produto.Nome,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario

            }).ToList()
        }).FirstOrDefaultAsync();

    }

    public async Task<bool> UpdateStatusPedido(int pedidoId, StatusPedido stt)
    {
        var pedido = await _context.Pedidos.FindAsync(pedidoId);
        if(pedido == null)
        {
            throw new Exception($"Não foi possivel encontrar o pedido {pedidoId}.");
        }
        if(stt == pedido.Status)
        {
            throw new Exception($"Não foi possivel alterar o status do pedido {pedidoId}.");
        }
        pedido.Status = stt;

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeletarPedidoPorID(int pedidoId)
    {   
        var pedido = await _context.Pedidos
        .Include(p => p.Itens)
        .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if(pedido == null) { return false; }

        foreach(var Item in pedido.Itens)
        {
            var produto = await _context.Produtos.FindAsync(Item.ProdutoId);
            if(produto != null)
            {
                produto.Estoque+= Item.Quantidade;
            }
        }

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();
        return true;
    }
    public PedidoRespostaDTO ConverterParaDTO(Pedido pedido)
    {
        return new PedidoRespostaDTO
        {
            Id = pedido.Id,
            NomeCliente = pedido.NomeCliente,
            DataPedido = pedido.DataPedido,
            ValorTotal = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario),
            Itens = pedido.Itens.Select(i => new ItemPedidoRespostaDTO
            {
                NomeProduto = i.Produto.Nome,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario

            }).ToList()
        };

         
    }
}