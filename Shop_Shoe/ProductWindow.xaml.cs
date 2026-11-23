using Microsoft.EntityFrameworkCore;
using Shop_Shoe;
using System;
using System.Linq;
using System.Windows;
using Shop_Shoe.Models;

namespace Shop_Shoe
{
    public partial class ProductWindow : Window
    {
        private ProductViewModel _product;
        private int _currentUserId;
        private bool _isAdminMode;

        public ProductWindow(ProductViewModel product, int userId)
        {
            InitializeComponent();
            _product = product;
            _currentUserId = userId;
            _isAdminMode = false;
            this.DataContext = _product;
            LoadSizes();
        }

        public ProductWindow(ProductViewModel product, bool isAdminMode)
        {
            InitializeComponent();
            _product = product;
            _isAdminMode = isAdminMode;
            this.DataContext = _product;
            OrderButton.Content = "Добавить в заказ";
            LoadSizes();
        }

        public CartItem SelectedCartItem { get; private set; }

        private void LoadSizes()
        {
            using (var db = new AppDbContext())
            {
                var availableSizes = db.StockItems.Include(si => si.Sizes)
                    .Where(si => si.ProductsId == _product.Id && si.Quantity > 0)
                    .Select(si => si.Sizes).ToList();
                SizeComboBox.ItemsSource = availableSizes;
                if (availableSizes.Any()) SizeComboBox.SelectedIndex = 0;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (SizeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите размер.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedSize = SizeComboBox.SelectedItem as Shop_Shoe.Models.Size;

            using (var db = new AppDbContext())
            {
                var stockItem = db.StockItems.FirstOrDefault(s => s.ProductsId == _product.Id && s.SizesId == selectedSize.Id);
                if (stockItem == null || stockItem.Quantity < quantity)
                {
                    int available = stockItem?.Quantity ?? 0;
                    MessageBox.Show($"Недостаточно товара на складе!\nДоступно: {available} шт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_isAdminMode)
                {
                    SelectedCartItem = new CartItem
                    {
                        ProductId = _product.Id,
                        SizeId = selectedSize.Id,
                        ProductName = _product.NameProduct,
                        SizeName = selectedSize.Size1.ToString(),
                        Price = _product.FinalPrice,
                        Quantity = quantity
                    };
                    this.DialogResult = true;
                    this.Close();
                    return;
                }

                try
                {
                    var newOrder = new Order { UserId = _currentUserId, DateOrder = DateTime.Now };
                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    var orderItem = new OrdersItem
                    {
                        OrdersId = newOrder.Id,
                        ProductsId = _product.Id,
                        SizeId = selectedSize.Id,
                        Quantity = quantity,
                        PricePerUnit = _product.FinalPrice
                    };
                    db.OrdersItems.Add(orderItem);
                    stockItem.Quantity -= quantity;

                    db.SaveChanges();
                    MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    if (ex.InnerException != null) errorMessage += "\n\nПодробности: " + ex.InnerException.Message;
                    MessageBox.Show($"Ошибка: {errorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}