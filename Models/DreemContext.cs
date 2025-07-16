using Microsoft.EntityFrameworkCore;

namespace Dreem.Models
{
    public class DreemContext : DbContext
    {
        public DreemContext(DbContextOptions<DreemContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<Kitchen> Kitchens { get; set; }
        public DbSet<Waiter> Waiters { get; set; }
        public DbSet<OrderProductItem> OrderProductItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Order <-> ProductItem join configuration
            modelBuilder.Entity<OrderProductItem>()
                .HasKey(opi => new { opi.OrderId, opi.ProductItemId });

            modelBuilder.Entity<OrderProductItem>()
                .HasOne(opi => opi.Order)
                .WithMany(o => o.OrderProductItems)
                .HasForeignKey(opi => opi.OrderId);

            modelBuilder.Entity<OrderProductItem>()
                .HasOne(opi => opi.ProductItem)
                .WithMany(pi => pi.OrderProductItems)
                .HasForeignKey(opi => opi.ProductItemId);

            // One-to-Many: Customer -> Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.OrdersRequested)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many: Kitchen -> Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Kitchen)
                .WithMany(k => k.Orders)
                .HasForeignKey(o => o.KitchenId)
                .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many: Waiter -> Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Waiter)
                .WithMany(w => w.OrdersCompleted)
                .HasForeignKey(o => o.WaiterId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
