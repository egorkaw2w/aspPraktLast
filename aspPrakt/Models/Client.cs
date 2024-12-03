using System;
using System.Collections.Generic;

namespace aspPrakt.Models;

public partial class Client
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? RoleId { get; set; }

    public DateTime? DateJoined { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }
}
