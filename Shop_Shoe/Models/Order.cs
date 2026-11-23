using System;
using System.Collections.Generic;

namespace Shop_Shoe.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime DateOrder { get; set; }

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();

    public virtual User User { get; set; } = null!;
}
