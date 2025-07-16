using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Dreem.Models;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly DreemContext _context;

        public EmailController(DreemContext context)
        {
            _context = context;
        }

        // GET: api/email/customer-id?email=someone@email.com
        [HttpGet("customer-id")]
        public async Task<IActionResult> GetCustomerIdByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var customer = await _context.Customers
                                         .Where(c => c.Email == email)
                                         .Select(c => new { c.Id })
                                         .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound("Customer not found.");

            return Ok(customer);
        }
    }
}
