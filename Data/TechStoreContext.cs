using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Models;
using System.Reflection.Emit;

namespace ProjetoTechStore_Volvo_2026.Data
{
    public class TechStoreContext : DbContext 
    {

        public TechStoreContext(DbContextOptions<TechStoreContext> options) : base(options)
        {
        }


        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<ItemPedido> ItensPedidos => Set<ItemPedido>();

        protected override void OnModelCreating(ModelBuilder construtorModelo)
        {
            base.OnModelCreating(construtorModelo);

            construtorModelo.Entity<ItemPedido>()
                .HasOne(ip => ip.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(ip => ip.PedidoId);

            construtorModelo.Entity<ItemPedido>()
                .HasOne(ip => ip.Produto)
                .WithMany()
                .HasForeignKey(ip => ip.ProdutoId);

            construtorModelo.Entity<Produto>()
                    .Property(p => p.Preco)
                    .HasColumnType("decimal(18,2)");

            construtorModelo.Entity<ItemPedido>()
                .Property(ip => ip.PrecoUnitario)
                .HasColumnType("decimal(18,2)");
        }
    }
}
