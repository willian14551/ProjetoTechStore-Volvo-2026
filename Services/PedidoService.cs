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

    public async Task<Pedido> CriarPedido(PedidoEntradaDTO pedido)
    {
        //utiliza-se de um DTO de entrada para criação, pois não é necessário ter todos os parâmetros sendo passados para o backend.
        //utiliza-se da estratégia para que o banco de dados possa fazer as operações caso ocorra a necessidade de um retry.
        var estrategiaExecucao = _context.Database.CreateExecutionStrategy();
        //metodo de retorno com todas as funções:
        return await estrategiaExecucao.ExecuteAsync(async () =>
        {
            //Começa a transação para que o banco de dados faça tudo de uma vez só.
            //metodo using para que o try catch fique mais otimizado, retornando quando deve e realizando Dispose() se necessário.

            using var transacao = await _context.Database.BeginTransactionAsync();
            //Tenta criar o pedido
            try
            {
                //Pedido criado dessa forma pois temos atributos required.
                Pedido ped = new Pedido
                {
                    NomeCliente = pedido.NomeCliente,
                    DataPedido = DateTime.Now,
                    Status = StatusPedido.PROCESSANDO,
                    Itens = new List<ItemPedido>()
                };

                //Roda a lista de pedidos, pois cada pedido pode ter mais de um produto envolvido.
                foreach(var produtoAux in pedido.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(produtoAux.ProdutoId);
                    //Se não achou o produto, joga uma exceção.
                    if(produto == null)
                    {
                        throw new Exception($"O produto com o id:{produtoAux.ProdutoId} não foi encontrado.");
                    }
                    //Exceção de erro de estoque.
                    if (produto.Estoque < produtoAux.Quantidade)
                    {
                        throw new Exception($"{produto.Nome} não possui estoque suficiente para realizar a transação.");
                    }

                    //reduz a quantidade.
                    produto.Estoque-=produtoAux.Quantidade;
                    //cria o item pedido, classe necessária para a relação entre pedido e produto.
                    var NovoItemPedido = new ItemPedido
                    {
                        ProdutoId = produto.Id,
                        Quantidade = produtoAux.Quantidade,
                        PrecoUnitario = produto.Preco,
                    };
                    ped.Itens.Add(NovoItemPedido);
                }
                //Após a criação do pedido, adiciona ao banco de dados e salva de forma assincrona.
                _context.Pedidos.Add(ped);
                //salva as mudanças realizadas no banco de dados.
                await _context.SaveChangesAsync();
                //finaliza a transação.
                await transacao.CommitAsync();
                //retorna o pedido para realização de operações no controller.
                return ped;
            }
            catch(Exception ex)
            {
                //caso dê erro, faça um rollback no banco de dados e exibe uma mensagem de erro.
                await transacao.RollbackAsync();
                var mensagemErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"Erro ao criar o pedido: {mensagemErro}.");
            }
        });
    }

    public async Task<List<PedidoRespostaDTO>> ListarPedidos()
    {
        //lista os pedidos de forma assincrona, utiliza-se do asnotracking pois é um método apenas de leitura.
        return await _context.Pedidos
        .AsNoTracking()
        .OrderByDescending(p => p.DataPedido) //ordena por ordem decrescente.
        .Select(p => new PedidoRespostaDTO //seleciona os pedidos e os converte em pedido resposta, necessário pois não queremos mostrar todas as informações do pedido.
        {
            Id = p.Id,
            DataPedido = p.DataPedido,
            NomeCliente = p.NomeCliente,
            ValorTotal = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade),
            stt = p.Status,
            Itens = p.Itens.Select(item => new ItemPedidoRespostaDTO //mesma ideia para item pedido, convertemos em dto de resposta.
            {
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                NomeProduto = item.Produto.Nome
            }).ToList() //to list para criar listas..
        }).ToListAsync(); 
    }

    public async Task<PedidoRespostaDTO?> ProcurarPedidoPorID(int pedidoId)
    {
        //mesma ideia do método anterior, apeans difere-se pela pesquisa por id e não listagem geral.
        return await _context.Pedidos
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

            }).ToList() //to list para criar listas..
        }).FirstOrDefaultAsync();

    }

    public async Task<bool> UpdateStatusPedido(int pedidoId, StatusPedido stt)
    {
        //este método apenas atualiza o status do pedido, pois atualizar o pedido em si não é necessário.
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

    //Em um negócio não faz sentido deletar os pedidos, mesmo que sejam pedidos fraudulentos ou pedidos errados, mas está aqui por método de debug. 
    public async Task<bool> DeletarPedidoPorID(int pedidoId)
    {   
        //faz a pesquisa do pedido por id:
        var pedido = await _context.Pedidos
        .Include(p => p.Itens)
        .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if(pedido == null) { return false; }

        //vai de item em item retornando o estoque, pois o pedido está "cancelado"
        foreach(var Item in pedido.Itens)
        {
            var produto = await _context.Produtos.FindAsync(Item.ProdutoId);
            if(produto != null)
            {
                produto.Estoque+= Item.Quantidade;
            }
        }

        _context.Pedidos.Remove(pedido);
        //salva as mudanças realizadas.
        await _context.SaveChangesAsync();
        return true;
    }

    //faz uma conversão, método auxiliar.
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