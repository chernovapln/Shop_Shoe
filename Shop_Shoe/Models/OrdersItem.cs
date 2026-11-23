using System;
using System.Collections.Generic;

namespace Shop_Shoe.Models;

public partial class OrdersItem
{
    public int Id { get; set; }

    public int OrdersId { get; set; }

    public int ProductsId { get; set; }

    public int SizeId { get; set; }

    public int? Quantity { get; set; }

    public decimal PricePerUnit { get; set; }

    public virtual Order Orders { get; set; } = null!;

    public virtual Product Products { get; set; } = null!;

    public virtual Size Size { get; set; } = null!;
}
