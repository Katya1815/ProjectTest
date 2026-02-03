using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restoran.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private List<Заказ> _allOrders;

        public OrdersPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                using (var context = App.Context)
                {
                    _allOrders = context.Заказы
                        .Include(o => o.Клиент)
                        .Include(o => o.Сотрудник)
                        .OrderByDescending(o => o.Дата_принятия_заказа)
                        .ThenByDescending(o => o.Время_принятия_заказа)
                        .ToList();

                    OrdersGrid.ItemsSource = _allOrders;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allOrders == null) return;

            var filtered = _allOrders.AsQueryable();

            // Поиск по номеру столика
            if (!string.IsNullOrWhiteSpace(SearchBox.Text) && int.TryParse(SearchBox.Text, out int tableNumber))
            {
                filtered = filtered.Where(o => o.Номер_столика == tableNumber);
            }

            // Фильтр по статусу
            if (StatusFilter.SelectedItem is ComboBoxItem statusItem && statusItem.Content.ToString() != "Все статусы")
            {
                string status = statusItem.Content.ToString();
                filtered = filtered.Where(o => o.Статус == status);
            }

            // Фильтр по дате
            if (DateFilter.SelectedDate.HasValue)
            {
                filtered = filtered.Where(o => o.Дата_принятия_заказа == DateFilter.SelectedDate.Value);
            }

            OrdersGrid.ItemsSource = filtered.ToList();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            StatusFilter.SelectedIndex = 0;
            DateFilter.SelectedDate = null;
            OrdersGrid.ItemsSource = _allOrders;
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                MessageBox.Show($"Просмотр деталей заказа #{orderId}", "Информация");
                // Можно открыть модальное окно с деталями
            }
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                MessageBox.Show($"Редактирование заказа #{orderId}", "Редактирование");
                // Навигация на страницу редактирования
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new AuthPage());
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderCreatePage());
        }
    }
}
