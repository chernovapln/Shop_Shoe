using Microsoft.EntityFrameworkCore;
using Shop_Shoe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Shop_Shoe.Models;

namespace Shop_Shoe
{
    public partial class AddOrderWindow : Window
    {
        private List<ProductViewModel> _allProducts;
        private List<CartItem> _currentOrderItems = new List<CartItem>();

        public AddOrderWindow()
        {
            InitializeComponent();
            LoadClients();
            LoadProducts();
            UpdateOrderSummary();
        }

        private void LoadClients()
        {
            using (var db = new AppDbContext())
            {
                var clients = db.Users.Select(u => new {
                    Id = u.Id,
                    FullName = u.Surname + " " + u.Name + " " + u.Patronymic
                }).ToList();
                ClientComboBox.ItemsSource = clients;
                if (clients.Any()) ClientComboBox.SelectedIndex = 0;
            }
        }

        private void LoadProducts()
        {
            using (var db = new AppDbContext())
            {
                var productsFromDb = db.Products.Include(p => p.Categories).Include(p => p.Manufacturers).Include(p => p.StockItems).ToList();
                _allProducts = new List<ProductViewModel>();

                foreach (var p in productsFromDb)
                {
                    var vm = new ProductViewModel(p);
                    vm.TotalQuantity = p.StockItems?.Sum(si => si.Quantity) ?? 0;
                    vm.FinalPrice = p.Price;
                    _allProducts.Add(vm);
                }
                ProductsDataGrid.ItemsSource = _allProducts;
            }
        }

        private void ProductSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ProductSearchTextBox.Text.ToLower().Trim();
            var filtered = _allProducts.Where(p => string.IsNullOrEmpty(searchText) || p.SearchText.Contains(searchText)).ToList();
            ProductsDataGrid.ItemsSource = filtered;
        }

        private void SelectProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button.DataContext as ProductViewModel;
            if (selectedProduct != null)
            {
                ProductWindow window = new ProductWindow(selectedProduct, true);
                if (window.ShowDialog() == true)
                {
                    var cartItem = window.SelectedCartItem;
                    if (cartItem != null)
                    {
                        _currentOrderItems.Add(cartItem);
                        UpdateOrderSummary();
                    }
                }
            }
        }

        private void UpdateOrderSummary()
        {
            CheckoutButton.Content = $"Оформить заказ ({_currentOrderItems.Count})";
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedItem == null || _currentOrderItems.Count == 0)
            {
                MessageBox.Show("Выберите клиента и добавьте хотя бы один товар.");
                return;
            }

            using (var db = new AppDbContext())
            {
                // Проверка остатков
                foreach (var item in _currentOrderItems)
                {
                    var stock = db.StockItems.FirstOrDefault(s => s.ProductsId == item.ProductId && s.SizesId == item.SizeId);
                    int available = stock?.Quantity ?? 0;
                    if (available < item.Quantity)
                    {
                        MessageBox.Show($"Недостаточно товара '{item.ProductName}' на складе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                try
                {
                    int clientId = (int)ClientComboBox.SelectedValue;
                    var newOrder = new Order { UserId = clientId, DateOrder = DateTime.Now };
                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    foreach (var item in _currentOrderItems)
                    {
                        db.OrdersItems.Add(new OrdersItem
                        {
                            OrdersId = newOrder.Id,
                            ProductsId = item.ProductId,
                            SizeId = item.SizeId,
                            Quantity = item.Quantity,
                            PricePerUnit = item.Price
                        });

                        var stock = db.StockItems.FirstOrDefault(s => s.ProductsId == item.ProductId && s.SizesId == item.SizeId);
                        if (stock != null) stock.Quantity -= item.Quantity;
                    }

                    db.SaveChanges();
                    MessageBox.Show("Заказ успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}