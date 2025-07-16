using Dreem.Models;

public class OrderProductItem
{
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int ProductItemId { get; set; }
    public ProductItem ProductItem { get; set; }

    public int Sequence { get; set; }
    public int Quantity { get; set; }
}
