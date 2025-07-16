namespace Dreem.Models
{
    public class Waiter
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ICollection<Order>? OrdersCompleted { get; set; }
    }
}
