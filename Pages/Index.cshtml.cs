using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ntsTexzd.data;

namespace ntsTexzd.Pages
{
    public class ProductsModel : PageModel
    {
        // ---------- ‘ильтры ----------
        [BindProperty(SupportsGet = true)]
        public int? Code { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? BarCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? Price { get; set; }

        // ---------- ѕагинаци€ ----------
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 25;   // значение по умолчанию

        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        private readonly ILogger<ProductsModel> _logger;

        private readonly ApplicationDbContext _db;

        public ProductsModel(ILogger<ProductsModel> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IList<Product> Products { get; set; } = new List<Product>();



        public async Task OnGetAsync()
        {

            IQueryable<Product> query = _db.Products
                   .Include(p => p.Price);

            // ---------- ‘ильтраци€ ----------
            if (Code.HasValue)
                query = query.Where(p => p.Code == Code.Value);

            if (!string.IsNullOrWhiteSpace(Name))
                query = query.Where(p => p.Name.Contains(Name));

            if (!string.IsNullOrWhiteSpace(BarCode))
                query = query.Where(p => p.BarCode.Contains(BarCode));

            if (Price.HasValue)
                query = query.Where(p => p.Price.PriceValue >= Price.Value);

            // ---------- ѕагинаци€ ----------
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            if (PageNumber < 1)
                PageNumber = 1;

            if (PageNumber > TotalPages && TotalPages > 0)
                PageNumber = TotalPages;

            const int maxVisiblePages = 5;

            StartPage = PageNumber - maxVisiblePages / 2;
            EndPage = PageNumber + maxVisiblePages / 2;

            if (StartPage < 1)
            {
                StartPage = 1;
                EndPage = Math.Min(maxVisiblePages, TotalPages);
            }

            if (EndPage > TotalPages)
            {
                EndPage = TotalPages;
                StartPage = Math.Max(1, TotalPages - maxVisiblePages + 1);
            }

            // ---------- ќЅя«ј“≈Ћ№Ќјя сортировка ----------
            query = query.OrderBy(p => p.Code);

            Products = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
