using Dreem.Models;
using Dreem.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductItemsController : ControllerBase
    {
        private readonly DreemContext _context;

        public ProductItemsController(DreemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductItemDto>>> GetProductItems()
        {
            var items = await _context.ProductItems
                .Include(p => p.OrderProductItems)
                .ThenInclude(opi => opi.Order)
                .Select(p => new ProductItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    OutOfStock = p.OutOfStock,
                    Type = p.Type,
                    OrderIds = p.OrderProductItems.Select(opi => opi.OrderId).ToList()
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductItemDto>> Get(int id)
        {
            var item = await _context.ProductItems
                .Include(p => p.OrderProductItems)
                .ThenInclude(opi => opi.Order)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null) return NotFound();

            var dto = new ProductItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
                OutOfStock = item.OutOfStock,
                Type = item.Type,
                OrderIds = item.OrderProductItems.Select(opi => opi.OrderId).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductItemDto>> Post(CreateProductItemDto dto)
        {
            var item = new ProductItem
            {
                Name = dto.Name,
                Category = dto.Category,
                OutOfStock = dto.OutOfStock,
                Type = dto.Type
            };

            _context.ProductItems.Add(item);
            await _context.SaveChangesAsync();

            
            var result = new ProductItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
                OutOfStock = item.OutOfStock,
                Type = item.Type,
                OrderIds = new List<int>() 
            };

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ProductItemDto dto)
        {
            if (id != dto.Id) return BadRequest("Mismatched ID.");

            var item = await _context.ProductItems
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null) return NotFound();

            item.Name = dto.Name;
            item.Category = dto.Category;
            item.OutOfStock = dto.OutOfStock;
            item.Type = dto.Type;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
