using Microsoft.EntityFrameworkCore;
using BaseIntroductionDotNetMentoring.Models;

namespace BaseIntroductionDotNetMentoring.Data
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Categories> Categories { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProductName).HasColumnName("ProductName").HasMaxLength(40);
                entity.Property(e => e.QuantityPerUnit).HasMaxLength(20);
                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId);

                entity.HasOne(p => p.Supplier)
                      .WithMany(s => s.Products)
                      .HasForeignKey(p => p.SupplierId);
            });

            modelBuilder.Entity<Categories>(cat =>
            {
                cat.ToTable("Categories");
                cat.HasKey(c => c.CategoryID);
                cat.Property(c => c.CategoryName).HasMaxLength(15);
                cat.Property(c => c.Description);
                cat.Property(c => c.Picture).HasColumnType("varbinary(max)");
            });

            modelBuilder.Entity<Supplier>(sup =>
            {
                sup.ToTable("Suppliers");
                sup.HasKey(s => s.SupplierID);
                sup.Property(s => s.CompanyName).HasMaxLength(40);
            });
        }
    }
}