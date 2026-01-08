using Microsoft.EntityFrameworkCore; 

namespace ntsTexzd.data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Price> Prices => Set<Price>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Таблица цен
            modelBuilder.Entity<Price>(entity =>
            {
                entity.ToTable("Prices");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.PriceValue)
                      .HasColumnName("Price")
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                // Индекс для поиска и сортировки по цене
                entity.HasIndex(e => e.PriceValue)
                      .HasDatabaseName("IX_Prices_Price");
            });

            // Таблица товаров
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.BarCode).HasMaxLength(100);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Model).HasMaxLength(100);
                entity.Property(e => e.Sort).HasMaxLength(100);
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.Weight).HasMaxLength(50);
                entity.Property(e => e.DateChanges).IsRequired();

                // Связь: Price (1) → Product (many)
                entity.HasOne(p => p.Price)
                      .WithMany(p => p.Products)
                      .HasForeignKey(p => p.IdPrice)
                      .OnDelete(DeleteBehavior.Restrict);
                // ---------- Индексы ----------
                entity.HasIndex(e => e.Code)
                      .HasDatabaseName("IX_Products_Code");

                entity.HasIndex(e => e.Name)
                      .HasDatabaseName("IX_Products_Name");

                entity.HasIndex(e => e.BarCode)
                      .HasDatabaseName("IX_Products_BarCode");
                // Поиск + сортировка
                entity.HasIndex(p => new { p.Name, p.Code });
                entity.HasIndex(p => new { p.Code, p.BarCode });

                // Сортировка через цену
                entity.HasIndex(p => new { p.IdPrice, p.Name });

                // Индекс на внешний ключ (ускоряет JOIN)
                entity.HasIndex(e => e.IdPrice)
                      .HasDatabaseName("IX_Products_IdPrice");
            });
        }
    }
}
