namespace Dreem.Models.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public int OrderCount { get; set; }
        public List<OrderSummaryDto>? Orders { get; set; }
    }

    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public bool Completed { get; set; }
    }
}
