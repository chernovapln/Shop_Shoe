using Microsoft.EntityFrameworkCore;
using Shop_Shoe;
using System;
using System.Linq;
using System.Windows;
using Shop_Shoe.Models;

namespace Shop_Shoe
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredLogin = LoginTextBox.Text.Trim();
            if (string.IsNullOrEmpty(enteredLogin))
            {
                ErrorTextBlock.Text = "Пожалуйста, введите логин.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            using (var db = new AppDbContext())
            {
                try
                {
                    var user = db.Users.Include(u => u.Roles).FirstOrDefault(u => u.Login == enteredLogin);
                    if (user == null)
                    {
                        ErrorTextBlock.Text = "Пользователь с таким логином не найден.";
                        ErrorTextBlock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.SetUserInfo(user.Id, user.Surname, user.Name, user.Patronymic, user.Roles.Name);
                        mainWindow.Show();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    ErrorTextBlock.Text = $"Ошибка подключения: {ex.Message}";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                }
            }
        }
    }
}