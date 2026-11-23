using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Shop_Shoe.Models;

namespace Shop_Shoe
{
    public partial class OrdersWindow : Window
    {
        private string _roleName;

        public OrdersWindow(string roleName, int userId)
        {
            InitializeComponent();
            _roleName = roleName;

            if (_roleName == "Администратор")
            {
                HeaderText.Text = "Все заказы (Администратор)";
                DeleteOrderButton.Visibility = Visibility.Visible;
            }
            else
            {
                HeaderText.Text = "Все заказы (Менеджер)";
            }

            LoadOrders();
        }

        private void LoadOrders()
        {
            using (var db = new AppDbContext())
            {
                var orders = db.Orders.Include(o => o.User).Include(o => o.OrdersItems)
                    .Select(o => new {
                        Id = o.Id,
                        DateOrder = o.DateOrder,
                        ClientName = $"{o.User.Surname} {o.User.Name} {o.User.Patronymic}",
                        TotalSum = o.OrdersItems.Sum(oi => oi.Quantity * oi.PricePerUnit)
                    }).OrderByDescending(o => o.DateOrder).ToList();
                OrdersDataGrid.ItemsSource = orders;
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedRow = OrdersDataGrid.SelectedItem;
            int orderId = (int)selectedRow.GetType().GetProperty("Id").GetValue(selectedRow, null);

            OrderDetailsWindow window = new OrderDetailsWindow(orderId, _roleName);
            window.ShowDialog();
            LoadOrders();
        }

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null) return;

            if (MessageBox.Show("Удалить заказ?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var selectedRow = OrdersDataGrid.SelectedItem;
                int orderId = (int)selectedRow.GetType().GetProperty("Id").GetValue(selectedRow, null);

                using (var db = new AppDbContext())
                {
                    var order = db.Orders.Include(o => o.OrdersItems).FirstOrDefault(o => o.Id == orderId);
                    if (order != null)
                    {
                        db.OrdersItems.RemoveRange(order.OrdersItems);
                        db.Orders.Remove(order);
                        db.SaveChanges();
                        LoadOrders();
                    }
                }
            }
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            AddOrderWindow window = new AddOrderWindow();
            window.ShowDialog();
            LoadOrders();
        }
    }
}