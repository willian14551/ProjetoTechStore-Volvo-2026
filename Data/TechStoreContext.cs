using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Models;
using System.Reflection.Emit;

namespace ProjetoTechStore_Volvo_2026.Data
{
    // Ponte entre o C# com o Banco
    public class TechStoreContext : DbContext 
    {

        // Construtor vazio para o asp.net passar as opções 
        public TechStoreContext(DbContextOptions<TechStoreContext> options) : base(options)
        {
        }

        // Tipo Dbset pra transformar as classes nas tabelas do banco pelo EF

        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<ItemPedido> ItensPedidos => Set<ItemPedido>();

        // Método OnModelCreating que é chamado pelo EF no começo
        // Tem função de setar os relacionamentos, chaves, constraints...
        protected override void OnModelCreating(ModelBuilder construtorModelo)
        {
            base.OnModelCreating(construtorModelo);

            // Configura a entidade ItemPedido -> Pedido (N Itens para 1 Pedido)
            construtorModelo.Entity<ItemPedido>()
                // ItemPedido tem 1 Pedido
                .HasOne(ip => ip.Pedido)
                // ItemPedido pode ter vários itens
                .WithMany(p => p.Itens)
                // FK utilizada é o ItemPedido.PedidoId (ou só ip.PedidoId)
                .HasForeignKey(ip => ip.PedidoId);

            // Configura a entidade ItemPedido -> Produto (N Itens para 1 Produto)
            construtorModelo.Entity<ItemPedido>()
                .HasOne(ip => ip.Produto)
                // Vazio para indicar que Produto NÃO precisa ter uma lista de Itens
                .WithMany()
                .HasForeignKey(ip => ip.ProdutoId);

            // Configura e especifica a quantia de casas decimais
            construtorModelo.Entity<Produto>()
                    .Property(p => p.Preco)
                    .HasColumnType("decimal(18,2)");

            // Faz o mesmo com o Precounitario do ItemPedido
            construtorModelo.Entity<ItemPedido>()
                .Property(ip => ip.PrecoUnitario)
                .HasColumnType("decimal(18,2)");
        }
    }
}
