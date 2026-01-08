using System;
using System.Collections.Generic;

namespace ntsTexzd.data
{
    public class Product
    {
        public Guid Id { get; set; }

        // Внешний ключ
        public Guid IdPrice { get; set; }

        public int Code { get; set; }
        public string Name { get; set; } = null!;
        public string BarCode { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string Model { get; set; } = null!;
        public string Sort { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Weight { get; set; } = null!;
        public DateTime DateChanges { get; set; }

        // Навигационное свойство (many → 1)
        public Price Price { get; set; } = null!;
    }
    public class ProductUpdateDto
    {
        public Guid Id { get; set; }
        public Guid PriceId { get; set; }   
        public decimal PriceValue { get; set; }

        public int Code { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public decimal Quantity { get; set; }
        public string Model { get; set; }
        public string Sort { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public DateTime DateChanges { get; set; }
    }
}
