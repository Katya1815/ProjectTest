using System;
using System.Windows;
using System.Windows.Controls;

namespace Restoran.Pages
{
    public partial class EmployeeMenuPage : Page
    {
        public EmployeeMenuPage()
        {
            InitializeComponent();
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new OrdersPage());
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MenuPage());
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new EmployeesPage());
        }

        // НОВЫЙ ОБРАБОТЧИК для кнопки "Смены"
        private void BtnShifts_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ShiftsPage());
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new UsersPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new AuthPage());

            var result = MessageBox.Show("Вы действительно хотите выйти?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigationService?.Navigate(new AuthPage());
            }
        }
    }
}