using System.Collections.Generic;

namespace Dreem.Models
{
    public class ProductItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public bool OutOfStock { get; set; }
        public string? Type { get; set; }

        public ICollection<OrderProductItem> OrderProductItems { get; set; } = new List<OrderProductItem>();
    }
}
