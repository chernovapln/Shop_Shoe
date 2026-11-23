using System;
using System.Collections.Generic;

namespace Shop_Shoe.Models;

public partial class Product
{
    public int Id { get; set; }

    public int CategoriesId { get; set; }

    public int SubcategoriesId { get; set; }

    public int ManufacturersId { get; set; }

    public string? Picture { get; set; }

    public string NameProduct { get; set; } = null!;

    public string? Description { get; set; }

    public string? Structure { get; set; }

    public decimal Price { get; set; }

    public virtual Kategory Categories { get; set; } = null!;

    public virtual Manufacturer Manufacturers { get; set; } = null!;

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();

    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();

    public virtual Subkategory Subcategories { get; set; } = null!;
}
