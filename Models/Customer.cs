

namespace Dreem.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string? Email { get; set; }

        public ICollection<Order>? OrdersRequested { get; set; }
    }
}
