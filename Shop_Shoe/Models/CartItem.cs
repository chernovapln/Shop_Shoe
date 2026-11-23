namespace Shop_Shoe
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}