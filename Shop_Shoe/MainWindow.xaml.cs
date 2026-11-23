using Microsoft.EntityFrameworkCore;
using Shop_Shoe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Shop_Shoe.Models;

namespace Shop_Shoe
{
    public partial class MainWindow : Window
    {
        private List<ProductViewModel> _allProducts;
        private List<ProductViewModel> _currentProducts;
        private int _currentUserId;
        private string _currentRole;

        public MainWindow()
        {
            InitializeComponent();
            LoadCatalog();
            LoadCategories();
        }

        public void SetUserInfo(int userId, string surname, string name, string patronymic, string roleName)
        {
            _currentUserId = userId;
            _currentRole = roleName;
            UserNameTextBlock.Text = $"{surname} {name} {patronymic}".Trim();

            // Показываем кнопку "Заказы" только Админу и Менеджеру
            if (roleName == "Администратор" || roleName == "Менеджер")
                OrdersButton.Visibility = Visibility.Visible;
            else
                OrdersButton.Visibility = Visibility.Collapsed;
        }

        private void LoadCatalog()
        {
            using (var db = new AppDbContext())
            {
                try
                {
                    var productsFromDb = db.Products.Include(p => p.Categories).Include(p => p.Manufacturers).Include(p => p.StockItems).ToList();
                    _allProducts = new List<ProductViewModel>();

                    foreach (var product in productsFromDb)
                    {
                        var viewModel = new ProductViewModel(product);
                        viewModel.TotalQuantity = product.StockItems?.Sum(si => si.Quantity) ?? 0;
                        bool hasOrders = CheckOrdersLastMonth(product.Id, db);
                        viewModel.FinalPrice = hasOrders ? product.Price : Math.Round(product.Price * 0.75m, 2);
                        _allProducts.Add(viewModel);
                    }

                    _currentProducts = new List<ProductViewModel>(_allProducts);
                    ProductsList.ItemsSource = _currentProducts;
                }
                catch (Exception ex) { MessageBox.Show($"Ошибка загрузки: {ex.Message}"); }
            }
        }

        private void LoadCategories()
        {
            if (_allProducts == null) return;
            var categories = _allProducts.Select(p => p.CategoryName).Distinct().OrderBy(c => c).ToList();
            categories.Insert(0, "Все категории");
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;
        }

        private bool CheckOrdersLastMonth(int productId, AppDbContext db)
        {
            DateTime firstDayLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            DateTime lastDayLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            return db.OrdersItems.Include(oi => oi.Orders).Any(oi => oi.ProductsId == productId && oi.Orders.DateOrder >= firstDayLastMonth && oi.Orders.DateOrder <= lastDayLastMonth);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            if (_allProducts == null) return;
            string searchText = SearchTextBox.Text.ToLower().Trim();
            var filtered = _allProducts.Where(p => string.IsNullOrEmpty(searchText) || p.SearchText.Contains(searchText)).ToList();

            string selectedCategory = CategoryComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Все категории")
                filtered = filtered.Where(p => p.CategoryName == selectedCategory).ToList();

            if (SortComboBox.SelectedIndex == 1) filtered = filtered.OrderBy(p => p.FinalPrice).ToList();
            else if (SortComboBox.SelectedIndex == 2) filtered = filtered.OrderByDescending(p => p.FinalPrice).ToList();

            _currentProducts = filtered;
            ProductsList.ItemsSource = _currentProducts;
        }

        private void ProductBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var selectedProduct = border.DataContext as ProductViewModel;
            if (selectedProduct != null)
            {
                ProductWindow window = new ProductWindow(selectedProduct, _currentUserId);
                window.ShowDialog();
                LoadCatalog();
                ApplyFilters();
            }
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow window = new OrdersWindow(_currentRole, _currentUserId);
            window.ShowDialog();
            LoadCatalog();
            ApplyFilters();
        }
    }
}