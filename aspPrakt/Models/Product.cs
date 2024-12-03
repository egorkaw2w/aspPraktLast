using System;
using System.Collections.Generic;

namespace aspPrakt.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int? CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? StockQuantity { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
