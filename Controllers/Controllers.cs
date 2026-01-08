using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;
using ntsTexzd.data; 

namespace ntsTexzd.Controllers
{
    [ApiController]
    [Route("api")]
    public class Controllers : Controller
    {
        private readonly ApplicationDbContext _context;

        public Controllers(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products.Include(p => p.Price).ToListAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _context.Products
                                   .Include(p => p.Price)
                                   .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductUpdateDto updated)
        {
            if (updated == null)
                return BadRequest();

            Product product = new Product();
            product.Id = Guid.NewGuid(); 

            // обновляем поля
            product.Name = updated.Name;
            product.Code = updated.Code;
            product.BarCode = updated.BarCode;
            product.Quantity = updated.Quantity;
            product.Model = updated.Model;
            product.Sort = updated.Sort;
            product.Color = updated.Color;
            product.Size = updated.Size;
            product.Weight = updated.Weight;
            product.DateChanges = updated.DateChanges;

            Price price= new Price();
            price.PriceValue = updated.PriceValue;
            price.Id = Guid.NewGuid();
            product.Price= price;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto updated)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            // обновляем поля
            product.Name = updated.Name;
            product.Code = updated.Code;
            product.BarCode = updated.BarCode;
            product.Quantity = updated.Quantity;
            product.Model = updated.Model;
            product.Sort = updated.Sort;
            product.Color = updated.Color;
            product.Size = updated.Size;
            product.Weight = updated.Weight;
            product.DateChanges = updated.DateChanges;
            // обновляем цену по PriceId
            if (product.Price == null || product.Price.Id != updated.PriceId)
            {
                product.Price = await _context.Prices.FindAsync(updated.PriceId);
            }
            product.Price.PriceValue = updated.PriceValue;

            await _context.SaveChangesAsync();
            return NoContent();
        }       
         
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
