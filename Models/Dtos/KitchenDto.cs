namespace Dreem.Models.Dtos
{
    public class KitchenDto
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public List<int> OrderIds { get; set; }
        public List<string> WaiterNames { get; set; }
    }
}
