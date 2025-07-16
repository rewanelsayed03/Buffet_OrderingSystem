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
    public class CustomersController : ControllerBase
    {
        private readonly DreemContext _context;

        public CustomersController(DreemContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> Get()
        {
            var customers = await _context.Customers
                .Include(c => c.OrdersRequested)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Email = c.Email,
                    OrderCount = c.OrdersRequested!.Count,
                    Orders = c.OrdersRequested.Select(o => new OrderSummaryDto
                    {
                        Id = o.Id,
                        Date = o.Date,
                        Description = o.Description,
                        Completed = o.Completed
                    }).ToList()
                })
                .ToListAsync();

            return Ok(customers);
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> Get(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.OrdersRequested)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            var dto = new CustomerDto
            {
                Id = customer.Id,
                Email = customer.Email,
                OrderCount = customer.OrdersRequested?.Count ?? 0,
                Orders = customer.OrdersRequested?.Select(o => new OrderSummaryDto
                {
                    Id = o.Id,
                    Date = o.Date,
                    Description = o.Description,
                    Completed = o.Completed
                }).ToList()
            };

            return Ok(dto);
        }

        // GET: api/customers/{id}/orders
        [HttpGet("{id}/orders")]
        public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetCustomerOrders(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.OrdersRequested)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            var orders = customer.OrdersRequested?.Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                Date = o.Date,
                Description = o.Description,
                Completed = o.Completed
            }).ToList();

            return Ok(orders);
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult> Post(CustomerDto customerDto)
        {
            if (string.IsNullOrWhiteSpace(customerDto.Email))
            {
                return BadRequest("Email is required.");
            }

            if (await _context.Customers.AnyAsync(c => c.Email == customerDto.Email))
            {
                return Conflict("A customer with this email already exists.");
            }

            var customer = new Customer
            {
                Email = customerDto.Email
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CustomerDto customerDto)
        {
            if (string.IsNullOrWhiteSpace(customerDto.Email))
            {
                return BadRequest("Email is required.");
            }

            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            if (await _context.Customers.AnyAsync(c => c.Id != id && c.Email == customerDto.Email))
            {
                return Conflict("Another customer with this email already exists.");
            }

            existingCustomer.Email = customerDto.Email;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "A concurrency error occurred while updating the customer.");
            }

            return NoContent();
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> CustomerExists(int id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }
    }
}
