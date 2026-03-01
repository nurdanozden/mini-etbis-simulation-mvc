using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniETBIS.Models;

namespace MiniETBIS.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Company>(entity =>
            {
                entity.HasIndex(c => c.TaxNumber).IsUnique();
                entity.HasIndex(c => c.City);
                entity.HasIndex(c => c.Sector);
                entity.HasOne(c => c.User)
                      .WithOne(u => u.Company)
                      .HasForeignKey<Company>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Company)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Sale>(entity =>
            {
                entity.HasIndex(s => s.SaleDate);
                entity.HasIndex(s => s.ProductId);
                entity.HasOne(s => s.Product)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(s => s.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
