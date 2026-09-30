using c1Soft_Projesi.Models;
using Microsoft.EntityFrameworkCore;

namespace c1Soft_Projesi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<SepetR> SepetR { get; set; }
    public DbSet<SepetD> SepetD { get; set; }
    public DbSet<SiparisR> SiparisR { get; set; }
    public DbSet<SiparisD> SiparisD { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<ProductGridColumn> ProductGridColumns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>()
            .HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId);

        modelBuilder.Entity<Product>()
            .HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId);

        modelBuilder.Entity<SepetR>()
            .HasOne(x => x.Kullanici)
            .WithMany()
            .HasForeignKey(x => x.KullaniciId);

        modelBuilder.Entity<SepetD>()
            .HasOne(x => x.Sepet)
            .WithMany(x => x.Kalemler)
            .HasForeignKey(x => x.SepetId);

        modelBuilder.Entity<SepetD>()
            .HasOne(x => x.Urun)
            .WithMany()
            .HasForeignKey(x => x.UrunId);

        modelBuilder.Entity<SiparisR>()
            .HasOne(x => x.Kullanici)
            .WithMany()
            .HasForeignKey(x => x.KullaniciId);

        modelBuilder.Entity<SiparisR>()
            .HasOne(x => x.Sepet)
            .WithMany()
            .HasForeignKey(x => x.SepetId)
            .IsRequired(false);

        modelBuilder.Entity<SiparisD>()
            .HasOne(x => x.Siparis)
            .WithMany(x => x.Kalemler)
            .HasForeignKey(x => x.SiparisId);

        modelBuilder.Entity<SiparisD>()
            .HasOne(x => x.Urun)
            .WithMany()
            .HasForeignKey(x => x.UrunId);

        modelBuilder.Entity<StockMovement>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

        modelBuilder.Entity<StockMovement>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false);
    }
}
