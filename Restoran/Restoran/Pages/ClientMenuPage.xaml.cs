using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RestoranApi.Models;

namespace Restoran.Pages
{
    public partial class ClientMenuPage : Page
    {
        private RestoranIs32Context _context;

        public ClientMenuPage()
        {
            InitializeComponent();
            _context = App.Context;
            LoadClientInfo();
        }

        private void LoadClientInfo()
        {
            if (App.CurrentClient != null)
            {
                var client = App.CurrentClient;
                TxtClientName.Text = $"{client.Фамилия} {client.Имя} {client.Отчество}";
                TxtRegistrationDate.Text = $"Дата регистрации: {client.ДатаРегистрацииВСистеме}";

                int ordersCount = _context.Заказs
                    .Count(z => z.Заказчик == client.IdКлиента);
                TxtOrdersCount.Text = $"Всего заказов: {ordersCount}";
            }
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел моих заказов", "Заказы",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnRestaurantMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Меню ресторана", "Блюда",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Создание нового заказа", "Заказ",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?",
                "Выход",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                App.CurrentClient = null;
                NavigationService?.Navigate(new AuthPage());
            }
        }
    }
}