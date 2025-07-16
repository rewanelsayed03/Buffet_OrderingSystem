namespace Dreem.Models.Dtos
{
    public class WaiterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<OrderDtoSimple> OrdersCompleted { get; set; }
    }

    public class OrderDtoSimple
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }
}
