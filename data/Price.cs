using System.Text.Json.Serialization;

namespace ntsTexzd.data
{
    public class Price
    {
        public Guid Id { get; set; }
        public decimal PriceValue { get; set; }

        [JsonIgnore]
        // Навигационное свойство (1 → many)
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
