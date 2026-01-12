using Microsoft.EntityFrameworkCore;
using StockMgmt.Core.Entities;

namespace StockMgmt.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username)
                .IsUnique();
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Category configurations
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Name)
                .IsUnique();
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Supplier configurations
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Product configurations
        modelBuilder.Entity<Product>(entity =>
        {
            // Note: SQLite doesn't support filtered indexes, so unique constraint applies to all rows
            // The query filter ensures soft-deleted items are excluded from queries
            entity.HasIndex(e => e.SKU)
                .IsUnique();
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Review configurations
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}

