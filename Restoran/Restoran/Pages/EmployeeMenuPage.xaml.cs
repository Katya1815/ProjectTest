using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace Restoran.Pages
{
    public partial class EmployeeMenuPage : Page
    {
        public EmployeeMenuPage()
        {
            InitializeComponent();

            // Проверка авторизации
            if (App.CurrentUser == null)
            {
                NavigationService?.Navigate(new AuthPage());
                return;
            }

            // Отображение информации о текущем пользователе
            DisplayUserInfo();
            LoadStatistics();
        }

        private void DisplayUserInfo()
        {
            try
            {
                // Полное имя сотрудника
                TxtEmployeeName.Text = $"{App.CurrentUser.Фамилия} {App.CurrentUser.Имя}";

                // Добавляем отчество, если оно есть
                if (!string.IsNullOrEmpty(App.CurrentUser.Отчество))
                {
                    TxtEmployeeName.Text += $" {App.CurrentUser.Отчество}";
                }

                // Получаем наименование должности через навигационное свойство
                if (App.CurrentUser.IdДолжностиNavigation != null)
                {
                    // ИСПРАВЛЕНО: НаименованиеДолжности вместо Название
                    TxtEmployeePosition.Text = App.CurrentUser.IdДолжностиNavigation.НаименованиеДолжности ?? "Сотрудник";
                }
                else
                {
                    // Если навигационное свойство не загружено, пытаемся загрузить отдельно
                    LoadPosition();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных пользователя: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtEmployeeName.Text = "Сотрудник";
                TxtEmployeePosition.Text = "Должность не указана";
            }
        }

        private void LoadPosition()
        {
            try
            {
                if (App.CurrentUser.IdДолжности.HasValue)
                {
                    // ИСПРАВЛЕНО: НаименованиеДолжности вместо Название
                    var position = App.Context.Должностиs
                        .FirstOrDefault(d => d.IdДолжности == App.CurrentUser.IdДолжности.Value);

                    TxtEmployeePosition.Text = position?.НаименованиеДолжности ?? "Должность не указана";
                }
                else
                {
                    TxtEmployeePosition.Text = "Должность не указана";
                }
            }
            catch
            {
                TxtEmployeePosition.Text = "Сотрудник";
            }
        }

        private void LoadStatistics()
        {
            try
            {
                // Получаем текущую дату для фильтрации заказов за сегодня
                var today = DateOnly.FromDateTime(DateTime.Today);

                // Подсчет заказов за сегодня
                int todayOrdersCount = App.Context.Заказs
                    .Count(o => o.ДатаПринятияЗаказа == today);

                TxtOrdersCount.Text = $"Заказов сегодня: {todayOrdersCount}";

                // Подсчет активных блюд
                int activeDishesCount = App.Context.Менюs.Count();
                TxtDishesCount.Text = $"Активных блюд: {activeDishesCount}";
            }
            catch (Exception ex)
            {
                TxtOrdersCount.Text = "Заказов сегодня: ошибка загрузки";
                TxtDishesCount.Text = "Активных блюд: ошибка загрузки";
            }
        }

        // Обработчик для кнопки "Заказы"
        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new OrdersPage());
        }

        // Обработчик для кнопки "Меню и блюда"
        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            // NavigationService?.Navigate(new MenuAndDishesPage());
            MessageBox.Show("Переход к меню и блюдам",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Обработчик для кнопки "Мои смены"
        private void BtnShifts_Click(object sender, RoutedEventArgs e)
        {
            // NavigationService?.Navigate(new ShiftsPage());
            MessageBox.Show("Переход к моим сменам",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Обработчик для кнопки "Клиенты"
        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            // NavigationService?.Navigate(new ClientsPage());
            MessageBox.Show("Переход к клиентам",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Обработчик для кнопки "Выйти"
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Подтверждение выхода
            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите выйти из системы?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Очищаем текущего пользователя
                App.CurrentUser = null;

                // Возвращаемся на страницу авторизации
                NavigationService?.Navigate(new AuthPage());
            }
        }
    }
}