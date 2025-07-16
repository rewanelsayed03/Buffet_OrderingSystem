namespace Dreem.Models
{
    public class Kitchen
    {
        public int Id { get; set; }
        public string? Location { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Waiter>? Waiters { get; set; }
    }
}
