using Microsoft.EntityFrameworkCore;
using ntsTexzd.data; 

namespace ntsTexzd
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDbContext _db;
                
        private const int RecordsCount = 1000;

        public DatabaseInitializer(ApplicationDbContext db)
        {
            _db = db;
        }
        public  void Initialize()
        {
            DeleteDatabaseIfExists();
            CreateAndSeedDatabase();
        }

        private  void DeleteDatabaseIfExists()
        {
            var dbPath = Path.GetFullPath(_db.Database.GetDbConnection().DataSource);

            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }

        private  void CreateAndSeedDatabase()
        { 
            // Отключаем отслеживание изменений
            _db.ChangeTracker.AutoDetectChangesEnabled = false;
            _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _db.Database.EnsureCreated();

            var random = new Random();

            // Создаем пул цен (например, 100 уникальных цен)
            var prices = GeneratePrices(random, count: 100);
            _db.Prices.AddRange(prices);
            _db.SaveChanges();

            // Создаем 1000 товаров
            var products = GenerateProducts(random, prices, RecordsCount);
            _db.Products.AddRange(products);
            _db.SaveChanges();
            // вернуть трекинг
            _db.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        private static List<Price> GeneratePrices(Random random, int count)
        {
            var prices = new List<Price>(count);

            for (int i = 0; i < count; i++)
            {
                prices.Add(new Price
                {
                    Id = Guid.NewGuid(),
                    PriceValue = Math.Round(
                        (decimal)(random.NextDouble() * 10_000 + 10), 2)
                });
            }

            return prices;
        }

        private static List<Product> GenerateProducts(
            Random random,
            List<Price> prices,
            int count)
        {
            var products = new List<Product>(count);

            string[] models = { "M1", "M2", "XL", "PRO", "MINI" };
            string[] colors = { "Черный", "Белый", "Красный", "Синий", "Зелёный" };
            string[] sorts = { "A", "B", "C" };
            string[] sizes = { "S", "M", "L", "XL" };

            for (int i = 0; i < count; i++)
            {
                var price = prices[random.Next(prices.Count)];

                products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    IdPrice = price.Id,

                    Code = random.Next(100000, 999999),
                    Name = $"Товар {i + 1}",
                    BarCode = random.NextInt64(1_000_000_000_000, 9_999_999_999_999).ToString(),
                    Quantity = Math.Round((decimal)(random.NextDouble() * 500), 2),

                    Model = models[random.Next(models.Length)],
                    Sort = sorts[random.Next(sorts.Length)],
                    Color = colors[random.Next(colors.Length)],
                    Size = sizes[random.Next(sizes.Length)],
                    Weight = $"{Math.Round(random.NextDouble() * 10 + 0.1, 2)} кг",

                    DateChanges = DateTime.UtcNow.AddMinutes(-random.Next(0, 100_000))
                });
            }

            return products;
        }
    }
}
