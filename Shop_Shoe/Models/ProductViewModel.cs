using Shop_Shoe.Models;
using System;
using Shop_Shoe.Models;

namespace Shop_Shoe
{
    public class ProductViewModel
    {
        public Product OriginalProduct { get; set; }
        public int Id { get; set; }
        public string NameProduct { get; set; }
        public string Structure { get; set; }
        public string CategoryName { get; set; }
        public string ManufacturerName { get; set; }
        public string PicturePath { get; set; }
        public string SearchText { get; set; }
        public decimal FinalPrice { get; set; }
        public int TotalQuantity { get; set; }

        public string QuantityStatus => TotalQuantity > 5 ? "Много" : "Мало";
        public string BackgroundColor => TotalQuantity <= 3 ? "#FF8080" : "Transparent";

        public ProductViewModel(Product product)
        {
            OriginalProduct = product;
            Id = product.Id;
            NameProduct = product.NameProduct;
            Structure = product.Structure;

            CategoryName = product.Categories?.Name ?? "Не указана";
            ManufacturerName = product.Manufacturers?.Name ?? "Не указан";

            PicturePath = string.IsNullOrEmpty(product.Picture)
                ? "pack://application:,,,/Images/picture.png"
                : $"pack://application:,,,/Images/{product.Picture}";

            SearchText = $"{NameProduct} {product.Description}".ToLower();
        }
    }
}