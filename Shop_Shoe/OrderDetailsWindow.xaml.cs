using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Shop_Shoe.Models;

namespace Shop_Shoe
{
    public partial class OrderDetailsWindow : Window
    {
        private int _orderId;
        private string _roleName;

        public OrderDetailsWindow(int orderId, string roleName)
        {
            InitializeComponent();
            _orderId = orderId;
            _roleName = roleName;
            OrderIdText.Text = orderId.ToString();

            if (_roleName == "Администратор")
            {
                OrderDatePicker.IsEnabled = true;
                SaveDateButton.Visibility = Visibility.Visible;
                DeleteColumn.Visibility = Visibility.Visible;
            }
            else
            {
                OrderDatePicker.IsEnabled = false;
                DeleteColumn.Visibility = Visibility.Collapsed;
            }

            LoadDetails();
        }

        private void LoadDetails()
        {
            using (var db = new AppDbContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == _orderId);
                if (order != null) OrderDatePicker.SelectedDate = order.DateOrder;

                var details = db.OrdersItems.Include(oi => oi.Products).Include(oi => oi.Size)
                    .Where(oi => oi.OrdersId == _orderId)
                    .Select(oi => new {
                        OrdersItemId = oi.Id,
                        ProductName = oi.Products.NameProduct,
                        SizeName = oi.Size.Size1.ToString(),
                        Quantity = oi.Quantity,
                        PricePerUnit = oi.PricePerUnit,
                        Total = oi.Quantity * oi.PricePerUnit
                    }).ToList();

                DetailsDataGrid.ItemsSource = details;
            }
        }

        private void SaveDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrderDatePicker.SelectedDate == null) return;
            using (var db = new AppDbContext())
            {
                try
                {
                    var order = db.Orders.FirstOrDefault(o => o.Id == _orderId);
                    if (order != null)
                    {
                        order.DateOrder = OrderDatePicker.SelectedDate.Value;
                        db.SaveChanges();
                        MessageBox.Show("Дата обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedRow = button.DataContext;

            int itemId = Convert.ToInt32(selectedRow.GetType().GetProperty("OrdersItemId").GetValue(selectedRow));

            if (MessageBox.Show("Удалить эту позицию из заказа?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    try
                    {
                        var itemToDelete = db.OrdersItems.FirstOrDefault(oi => oi.Id == itemId);
                        if (itemToDelete != null)
                        {
                            var stock = db.StockItems.FirstOrDefault(s => s.ProductsId == itemToDelete.ProductsId && s.SizesId == itemToDelete.SizeId);

                            if (stock != null)
                                stock.Quantity = stock.Quantity + (itemToDelete.Quantity ?? 0);

                            db.OrdersItems.Remove(itemToDelete);
                            db.SaveChanges();
                            LoadDetails();
                        }
                    }
                    catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
                }
            }
        }
    }
}