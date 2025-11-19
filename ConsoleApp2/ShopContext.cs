using ConsoleApp2.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp2;

// DbContext = "enheten" representerar databasen
public class ShopContext : DbContext
{
    // Db<Category> mappar till tabellen i Category i databasen
    public DbSet<Category> Categories => Set<Category>();
    // Db<Product> mappar till 
    public DbSet<Product> Products => Set<Product>();
    
    // Här berättar vi för EF Core att vi vill använda SQLite och var filen ska ligga
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "shop.db");
        
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }
    // OnModelCreating används för att finjustera modellen

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(e =>
        {
            // Sätter primär nyckel
            e.HasKey(x => x.CategoryId);

            // Säkerställer samma regler som data annotatuibs ( required + MaxLength)
            e.Property(x => x.CategoryName)
                .IsRequired().HasMaxLength(100);

            e.Property(x => x.CategoryDescription).HasMaxLength(250);
            
            // Skapar ett UNIQUE - index på CategoryName
            // Databasen tillåter inte två rader med samma CategoryName.
            // Kanske inte vill ha två kategorier som heter "Books"
            e.HasIndex(x => x.CategoryName).IsUnique();
        });
        
        modelBuilder.Entity<Product>(e =>
        {
            // Primärnyckel
            e.HasKey(product => product.ProductId);
            e.Property(product => product.Pris)
                .IsRequired();
            e.Property(product => product.Name)
                .IsRequired().HasMaxLength(100);
            e.Property(product => product.Description)
                .HasMaxLength(250);
            e.HasIndex(product => product.Name)
                .IsUnique();
            e.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
    }
}