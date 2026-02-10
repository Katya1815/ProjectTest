using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class ClientsPage : Page
    {
        private List<Клиент> _allClients;
        private List<Заказ> _allOrders;

        public class ClientViewModel
        {
            public int Id_клиента { get; set; }
            public string Фамилия { get; set; }
            public string Имя { get; set; }
            public string Отчество { get; set; }
            public string Номер_телефона { get; set; }
            public string Email { get; set; }
            public DateTime Дата_регистрации { get; set; }
            public string Примечания { get; set; }
            public int Количество_заказов { get; set; }
            public decimal Общая_сумма_заказов { get; set; }
            public string Статус
            {
                get
                {
                    if (Количество_заказов == 0) return "Новый";
                    if (Количество_заказов >= 10) return "Постоянный";
                    if (Количество_заказов >= 5) return "Активный";
                    return "Обычный";
                }
            }
        }

        public ClientsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                using (var context = App.Context)
                {
                    // Загружаем клиентов
                    _allClients = context.Клиенты
                        .OrderBy(c => c.Фамилия)
                        .ThenBy(c => c.Имя)
                        .ToList();

                    // Загружаем заказы для статистики
                    _allOrders = context.Заказы.ToList();

                    // Создаем ViewModel с дополнительной статистикой
                    var clientViewModels = _allClients.Select(client => new ClientViewModel
                    {
                        Id_клиента = client.Id_клиента,
                        Фамилия = client.Фамилия,
                        Имя = client.Имя,
                        Отчество = client.Отчество,
                        Номер_телефона = client.Номер_телефона ?? "Не указан",
                        Email = client.Email ?? "Не указан",
                        Дата_регистрации = client.Дата_регистрации,
                        Примечания = client.Примечания ?? "",
                        Количество_заказов = _allOrders.Count(o => o.Id_клиента == client.Id_клиента),
                        Общая_сумма_заказов = _allOrders
                            .Where(o => o.Id_клиента == client.Id_клиента)
                            .Sum(o => o.Сумма)
                    }).ToList();

                    ClientsDataGrid.ItemsSource = clientViewModels;
                    UpdateStatistics(clientViewModels);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics(List<ClientViewModel> clients)
        {
            if (clients == null) return;

            var totalClients = clients.Count;
            var clientsWithOrders = clients.Count(c => c.Количество_заказов > 0);
            var totalOrders = clients.Sum(c => c.Количество_заказов);
            var totalRevenue = clients.Sum(c => c.Общая_сумма_заказов);
            var avgOrders = totalClients > 0 ? (double)totalOrders / totalClients : 0;

            StatisticsText.Text = $"С заказами: {clientsWithOrders} | Всего заказов: {totalOrders} | Ср. заказов: {avgOrders:F1}";
            RevenueText.Text = $"Общий доход: {totalRevenue:C}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allClients == null) return;

            var clientViewModels = GetClientViewModels();

            // Фильтрация по поиску
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                clientViewModels = clientViewModels.Where(client =>
                    client.Фамилия.ToLower().Contains(searchText) ||
                    client.Имя.ToLower().Contains(searchText) ||
                    client.Отчество.ToLower().Contains(searchText) ||
                    client.Номер_телефона.ToLower().Contains(searchText) ||
                    client.Примечания.ToLower().Contains(searchText)).ToList();
            }

            // Фильтр "Только с заказами"
            if (ShowOnlyWithOrdersCheckBox.IsChecked == true)
            {
                clientViewModels = clientViewModels.Where(c => c.Количество_заказов > 0).ToList();
            }

            // Сортировка
            if (SortComboBox.SelectedItem is ComboBoxItem sortItem)
            {
                switch (sortItem.Content.ToString())
                {
                    case "По ФИО (А-Я)":
                        clientViewModels = clientViewModels
                            .OrderBy(c => c.Фамилия)
                            .ThenBy(c => c.Имя)
                            .ThenBy(c => c.Отчество)
                            .ToList();
                        break;
                    case "По ФИО (Я-А)":
                        clientViewModels = clientViewModels
                            .OrderByDescending(c => c.Фамилия)
                            .ThenByDescending(c => c.Имя)
                            .ThenByDescending(c => c.Отчество)
                            .ToList();
                        break;
                    case "По дате регистрации":
                        clientViewModels = clientViewModels
                            .OrderByDescending(c => c.Дата_регистрации)
                            .ToList();
                        break;
                    case "По количеству заказов":
                        clientViewModels = clientViewModels
                            .OrderByDescending(c => c.Количество_заказов)
                            .ToList();
                        break;
                }
            }

            ClientsDataGrid.ItemsSource = clientViewModels;
            UpdateStatistics(clientViewModels);
        }

        private List<ClientViewModel> GetClientViewModels()
        {
            return _allClients.Select(client => new ClientViewModel
            {
                Id_клиента = client.Id_клиента,
                Фамилия = client.Фамилия,
                Имя = client.Имя,
                Отчество = client.Отчество,
                Номер_телефона = client.Номер_телефона ?? "Не указан",
                Email = client.Email ?? "Не указан",
                Дата_регистрации = client.Дата_регистрации,
                Примечания = client.Примечания ?? "",
                Количество_заказов = _allOrders.Count(o => o.Id_клиента == client.Id_клиента),
                Общая_сумма_заказов = _allOrders
                    .Where(o => o.Id_клиента == client.Id_клиента)
                    .Sum(o => o.Сумма)
            }).ToList();
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Форма добавления клиента в разработке", "Информация");
            // NavigationService.Navigate(new AddEditClientPage());
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int clientId)
            {
                var client = _allClients.FirstOrDefault(c => c.Id_клиента == clientId);
                if (client != null)
                {
                    MessageBox.Show($"Редактирование клиента: {client.Фамилия} {client.Имя}",
                        "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
                    // NavigationService.Navigate(new AddEditClientPage(clientId));
                }
            }
        }

        private void ViewOrdersHistory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int clientId)
            {
                var client = _allClients.FirstOrDefault(c => c.Id_клиента == clientId);
                if (client != null)
                {
                    var ordersCount = _allOrders.Count(o => o.Id_клиента == clientId);
                    MessageBox.Show($"История заказов клиента: {client.Фамилия} {client.Имя}\n" +
                                   $"Всего заказов: {ordersCount}",
                                   "История заказов", MessageBoxButton.OK, MessageBoxImage.Information);
                    // NavigationService.Navigate(new ClientOrdersPage(clientId));
                }
            }
        }

        private void CallClient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string phoneNumber && phoneNumber != "Не указан")
            {
                MessageBox.Show($"Имитация звонка клиенту: {phoneNumber}", "Звонок");
                // Реальная логика звонка
            }
            else
            {
                MessageBox.Show("У клиента не указан номер телефона", "Ошибка");
            }
        }

        private void EmailClient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string email && email != "Не указан")
            {
                MessageBox.Show($"Имитация отправки email на адрес: {email}", "Email");
                // Реальная логика отправки email
            }
            else
            {
                MessageBox.Show("У клиента не указан email", "Ошибка");
            }
        }

        private void ClientsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is ClientViewModel selectedClient)
            {
                EditClient_Click(sender, new RoutedEventArgs());
            }
        }

        private void ShowFilters_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Расширенные фильтры в разработке", "Фильтры");
        }

        private void ImportClients_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Импорт клиентов в разработке", "Импорт");
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Генерация отчета в разработке", "Отчет");
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            SortComboBox.SelectedIndex = 0;
            ShowOnlyWithOrdersCheckBox.IsChecked = false;

            var clientViewModels = GetClientViewModels();
            ClientsDataGrid.ItemsSource = clientViewModels;
            UpdateStatistics(clientViewModels);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new AdminMenuPage());
        }
    }

    // Конвертеры для WPF
    public class OrdersCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count == 0 ? "0" : $"{count}";
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CurrencyColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                return amount.ToString("C", culture);
            }
            return "0 ₽";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status)
                {
                    case "Постоянный":
                        return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
                    case "Активный":
                        return new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Синий
                    case "Обычный":
                        return new SolidColorBrush(Color.FromRgb(158, 158, 158)); // Серый
                    case "Новый":
                        return new SolidColorBrush(Color.FromRgb(156, 39, 176)); // Фиолетовый
                    default:
                        return new SolidColorBrush(Color.FromRgb(189, 189, 189));
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}