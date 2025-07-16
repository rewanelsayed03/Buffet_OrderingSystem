using System;
using System.Collections.Generic;

namespace Dreem.Models.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public bool Completed { get; set; }
        public string Status { get; set; }

        public string CustomerEmail { get; set; }
        public string KitchenLocation { get; set; }
        public string WaiterName { get; set; }

        public List<OrderProductItemDto> ProductItems { get; set; }
    }

    public class OrderProductItemDto
    {
        public string ProductItemName { get; set; }
        public int Sequence { get; set; }
        public int Quantity { get; set; }
    }
}
