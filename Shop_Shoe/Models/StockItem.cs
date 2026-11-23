using System;
using System.Collections.Generic;

namespace Shop_Shoe.Models;

public partial class StockItem
{
    public int Id { get; set; }

    public int ProductsId { get; set; }

    public int SizesId { get; set; }

    public int Quantity { get; set; }

    public virtual Product Products { get; set; } = null!;

    public virtual Size Sizes { get; set; } = null!;
}
