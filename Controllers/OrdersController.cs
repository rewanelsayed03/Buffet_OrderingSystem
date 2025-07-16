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
    public class OrdersController : ControllerBase
    {
        private readonly DreemContext _context;

        public OrdersController(DreemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Kitchen)
                .Include(o => o.Waiter)
                .Include(o => o.OrderProductItems)
                    .ThenInclude(opi => opi.ProductItem)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    Date = o.Date,
                    Description = o.Description,
                    Note = o.Note,
                    Completed = o.Completed,
                    Status = o.Status.ToString(),
                    CustomerEmail = o.Customer.Email,
                    KitchenLocation = o.Kitchen.Location,
                    WaiterName = o.Waiter.Name,
                    ProductItems = o.OrderProductItems
                        .OrderBy(opi => opi.Sequence)
                        .Select(opi => new OrderProductItemDto
                        {
                            ProductItemName = opi.ProductItem.Name,
                            Sequence = opi.Sequence,
                            Quantity = opi.Quantity
                        }).ToList()
                }).ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> Get(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Kitchen)
                .Include(o => o.Waiter)
                .Include(o => o.OrderProductItems)
                    .ThenInclude(opi => opi.ProductItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var dto = new OrderDto
            {
                Id = order.Id,
                Date = order.Date,
                Description = order.Description,
                Note = order.Note,
                Completed = order.Completed,
                Status = order.Status.ToString(),
                CustomerEmail = order.Customer.Email,
                KitchenLocation = order.Kitchen.Location,
                WaiterName = order.Waiter.Name,
                ProductItems = order.OrderProductItems
                    .OrderBy(opi => opi.Sequence)
                    .Select(opi => new OrderProductItemDto
                    {
                        ProductItemName = opi.ProductItem.Name,
                        Sequence = opi.Sequence,
                        Quantity = opi.Quantity
                    }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Post(OrderDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.CustomerEmail);
            var kitchen = await _context.Kitchens.FirstOrDefaultAsync(k => k.Location == dto.KitchenLocation);
            var waiter = await _context.Waiters.FirstOrDefaultAsync(w => w.Name == dto.WaiterName);

            if (customer == null) return BadRequest("Invalid CustomerEmail.");
            if (kitchen == null) return BadRequest("Invalid KitchenLocation.");
            if (waiter == null) return BadRequest("Invalid WaiterName.");

            if (!Enum.TryParse<OrderStatus>(dto.Status, out var parsedStatus))
                return BadRequest("Invalid Status.");

            var order = new Order
            {
                Date = dto.Date,
                Description = dto.Description,
                Note = dto.Note,
                Completed = dto.Completed,
                Status = parsedStatus,
                CustomerId = customer.Id,
                KitchenId = kitchen.Id,
                WaiterId = waiter.Id,
                OrderProductItems = new List<OrderProductItem>()
            };

            if (dto.ProductItems != null)
            {
                foreach (var itemDto in dto.ProductItems)
                {
                    var product = await _context.ProductItems.FirstOrDefaultAsync(p => p.Name == itemDto.ProductItemName);
                    if (product == null) return BadRequest($"Invalid ProductItemName: {itemDto.ProductItemName}");

                    order.OrderProductItems.Add(new OrderProductItem
                    {
                        ProductItemId = product.Id,
                        Sequence = itemDto.Sequence,
                        Quantity = itemDto.Quantity
                    });
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            dto.Id = order.Id;
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, OrderDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");

            var order = await _context.Orders
                .Include(o => o.OrderProductItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.CustomerEmail);
            var kitchen = await _context.Kitchens.FirstOrDefaultAsync(k => k.Location == dto.KitchenLocation);
            var waiter = await _context.Waiters.FirstOrDefaultAsync(w => w.Name == dto.WaiterName);

            if (customer == null || kitchen == null || waiter == null)
                return BadRequest("Invalid related data.");

            if (!Enum.TryParse<OrderStatus>(dto.Status, out var parsedStatus))
                return BadRequest("Invalid Status.");

            order.Date = dto.Date;
            order.Description = dto.Description;
            order.Note = dto.Note;
            order.Completed = dto.Completed;
            order.Status = parsedStatus;
            order.CustomerId = customer.Id;
            order.KitchenId = kitchen.Id;
            order.WaiterId = waiter.Id;

            // Replace existing product items
            order.OrderProductItems.Clear();
            if (dto.ProductItems != null)
            {
                foreach (var itemDto in dto.ProductItems)
                {
                    var product = await _context.ProductItems.FirstOrDefaultAsync(p => p.Name == itemDto.ProductItemName);
                    if (product == null) return BadRequest($"Invalid ProductItemName: {itemDto.ProductItemName}");

                    order.OrderProductItems.Add(new OrderProductItem
                    {
                        OrderId = order.Id,
                        ProductItemId = product.Id,
                        Sequence = itemDto.Sequence,
                        Quantity = itemDto.Quantity
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
