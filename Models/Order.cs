using System;
using System.Collections.Generic;

namespace Dreem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public bool Completed { get; set; }
        public OrderStatus Status { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int KitchenId { get; set; }
        public Kitchen Kitchen { get; set; }

        public int WaiterId { get; set; }
        public Waiter Waiter { get; set; }

        public ICollection<OrderProductItem> OrderProductItems { get; set; } = new List<OrderProductItem>();
    }

    public enum OrderStatus
    {
        Pending,
        InProgress,
        Ready,
        Served
    }
}
