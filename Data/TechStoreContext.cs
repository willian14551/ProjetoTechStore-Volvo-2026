using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Models;

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
        protected override void OnModelCreating(ModelBuilder ConstrutorModelo)
        {
            base.OnModelCreating(ConstrutorModelo);

            // Configura a entidade ItemPedido -> Pedido (N Itens para 1 Pedido)
            ConstrutorModelo.Entity<ItemPedido>()
                // ItemPedido tem 1 Pedido
                .HasOne(ip => ip.Pedido)
                // ItemPedido pode ter vários itens
                .WithMany(p => p.Itens)
                // FK utilizada é o ItemPedido.PedidoId (ou só ip.PedidoId)
                .HasForeignKey(ip => ip.PedidoId);

            // Configura a entidade ItemPedido -> Produto (N Itens para 1 Produto)
            ConstrutorModelo.Entity<ItemPedido>()
                .HasOne(ip => ip.Produto)
                // Vazio para indicar que Produto NÃO precisa ter uma lista de Itens
                .WithMany()
                .HasForeignKey(ip => ip.ProdutoId);
        }
    }
}
