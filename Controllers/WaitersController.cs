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
    public class WaitersController : ControllerBase
    {
        private readonly DreemContext _context;

        public WaitersController(DreemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaiterDto>>> Get()
        {
            var waiters = await _context.Waiters
                .Include(w => w.OrdersCompleted)
                .Select(w => new WaiterDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    OrdersCompleted = w.OrdersCompleted.Select(o => new OrderDtoSimple
                    {
                        Id = o.Id,
                        Date = o.Date,
                        Description = o.Description,
                        Completed = o.Completed
                    }).ToList()
                })
                .ToListAsync();

            return Ok(waiters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WaiterDto>> Get(int id)
        {
            var waiter = await _context.Waiters
                .Include(w => w.OrdersCompleted)
                .Select(w => new WaiterDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    OrdersCompleted = w.OrdersCompleted.Select(o => new OrderDtoSimple
                    {
                        Id = o.Id,
                        Date = o.Date,
                        Description = o.Description,
                        Completed = o.Completed
                    }).ToList()
                })
                .FirstOrDefaultAsync(w => w.Id == id);

            if (waiter == null) return NotFound();
            return Ok(waiter);
        }

        [HttpPost]
        public async Task<ActionResult<WaiterDto>> Post(WaiterDto dto)
        {
            var waiter = new Waiter
            {
                Name = dto.Name
            };

            _context.Waiters.Add(waiter);
            await _context.SaveChangesAsync();

            dto.Id = waiter.Id;
            dto.OrdersCompleted = new List<OrderDtoSimple>();

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, WaiterDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var existing = await _context.Waiters.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var waiter = await _context.Waiters.FindAsync(id);
            if (waiter == null) return NotFound();

            _context.Waiters.Remove(waiter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> WaiterExists(int id)
        {
            return await _context.Waiters.AnyAsync(w => w.Id == id);
        }
    }
}
