using System;
using System.Collections.Generic;

namespace Shop_Shoe.Models;

public partial class User
{
    public int Id { get; set; }

    public int RolesId { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Login { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Roles { get; set; } = null!;
}
