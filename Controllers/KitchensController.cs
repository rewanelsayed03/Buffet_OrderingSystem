using Dreem.Models;
using Dreem.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenController : ControllerBase
    {
        private readonly DreemContext _context;

        public KitchenController(DreemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KitchenDto>>> Get()
        {
            var kitchens = await _context.Kitchens
                .Include(k => k.Orders)
                .Include(k => k.Waiters)
                .Select(k => new KitchenDto
                {
                    Id = k.Id,
                    Location = k.Location,
                    OrderIds = k.Orders.Select(o => o.Id).ToList(),
                    WaiterNames = k.Waiters.Select(w => w.Name).ToList()
                })
                .ToListAsync();

            return Ok(kitchens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KitchenDto>> Get(int id)
        {
            var kitchen = await _context.Kitchens
                .Include(k => k.Orders)
                .Include(k => k.Waiters)
                .Where(k => k.Id == id)
                .Select(k => new KitchenDto
                {
                    Id = k.Id,
                    Location = k.Location,
                    OrderIds = k.Orders.Select(o => o.Id).ToList(),
                    WaiterNames = k.Waiters.Select(w => w.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (kitchen == null) return NotFound();

            return Ok(kitchen);
        }

        [HttpPost]
        public async Task<ActionResult<KitchenDto>> Post(KitchenDto dto)
        {
            var kitchen = new Kitchen
            {
                Location = dto.Location,
                Orders = new List<Order>(),    
                Waiters = new List<Waiter>()
            };

            _context.Kitchens.Add(kitchen);
            await _context.SaveChangesAsync();

            dto.Id = kitchen.Id;
            dto.OrderIds = new List<int>();
            dto.WaiterNames = new List<string>();

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, KitchenDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var kitchen = await _context.Kitchens.FindAsync(id);
            if (kitchen == null) return NotFound();

            kitchen.Location = dto.Location;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kitchen = await _context.Kitchens.FindAsync(id);
            if (kitchen == null) return NotFound();

            _context.Kitchens.Remove(kitchen);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> KitchenExists(int id)
        {
            return await _context.Kitchens.AnyAsync(k => k.Id == id);
        }
    }
}
