using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RestoranApi.Models;

namespace Restoran.Pages
{
    public partial class EmployeeMenuPage : Page
    {
        private RestoranIs32Context _context;

        public EmployeeMenuPage()
        {
            InitializeComponent();
            _context = App.Context;
            LoadEmployeeInfo();
            LoadStatistics();
        }

        private void LoadEmployeeInfo()
        {
            if (App.CurrentUser != null)
            {
                var employee = App.CurrentUser;
                TxtEmployeeName.Text = $"{employee.Фамилия} {employee.Имя} {employee.Отчество}";

                var position = _context.Должностиs
                    .FirstOrDefault(d => d.IdДолжности == employee.IdДолжности);

                if (position != null)
                {
                    TxtEmployeePosition.Text = $"Должность: {position.НаименованиеДолжности}";
                }
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                int ordersCount = _context.Заказs
                    .Count(z => z.ДатаПринятияЗаказа == today);
                TxtOrdersCount.Text = $"Заказов сегодня: {ordersCount}";

                int dishesCount = _context.Менюs
                    .Count(m => m.ДоступноДляЗаказа == true);
                TxtDishesCount.Text = $"Активных блюд: {dishesCount}";
            }
            catch (Exception ex)
            {
                TxtOrdersCount.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел управления заказами", "Заказы",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел управления меню", "Меню",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnShifts_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел моих смен", "Смены",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел клиентов", "Клиенты",
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
                App.CurrentUser = null;
                NavigationService?.Navigate(new AuthPage());
            }
        }
    }
}