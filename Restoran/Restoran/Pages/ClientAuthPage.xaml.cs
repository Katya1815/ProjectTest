using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RestoranApi.Models;

namespace Restoran.Pages
{
    public partial class ClientAuthPage : Page
    {
        public ClientAuthPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string lastName = TBoxLastName.Text.Trim();
            string firstName = TBoxFirstName.Text.Trim();

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка!",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var client = App.Context.Клиентs
                .FirstOrDefault(c => c.Фамилия == lastName && c.Имя == firstName);

            if (client == null)
            {
                MessageBox.Show("Клиент не найден! Проверьте введенные данные.",
                    "Ошибка входа!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                TBoxLastName.Clear();
                TBoxFirstName.Clear();
                return;
            }

            App.CurrentClient = client;

            MessageBox.Show($"Добро пожаловать, {client.Имя}!",
                "Успешный вход!",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            NavigationService?.Navigate(new ClientMenuPage());

            TBoxLastName.Clear();
            TBoxFirstName.Clear();
        }

        private void BtnEmployee_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AuthPage());
        }
    }
}