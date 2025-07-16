public class ProductItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public bool OutOfStock { get; set; }
    public string Type { get; set; }

    public List<int> OrderIds { get; set; }
}
