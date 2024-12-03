using System;
using System.Collections.Generic;

namespace aspPrakt.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Client? User { get; set; }
}
