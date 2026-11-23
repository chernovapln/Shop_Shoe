using System;
using System.Collections.Generic;

namespace Shop_Shoe.Models;

public partial class Size
{
    public int Id { get; set; }

    public decimal Size1 { get; set; }

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();

    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
