using System;
using System.Collections.Generic;

namespace aspPrakt.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
