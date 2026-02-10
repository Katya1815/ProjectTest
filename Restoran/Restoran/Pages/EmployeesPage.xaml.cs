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
    public partial class EmployeesPage : Page
    {
        private List<Сотрудник> _allEmployees;
        private List<Должности> _positions;

        public EmployeesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
            LoadPositions();
        }

        private void LoadEmployees()
        {
            try
            {
                using (var context = App.Context)
                {
                    _allEmployees = context.Сотрудники
                        .Include(e => e.Должность)
                        .Include(e => e.Роль)
                        .OrderBy(e => e.Фамилия)
                        .ThenBy(e => e.Имя)
                        .ToList();

                    EmployeesDataGrid.ItemsSource = _allEmployees;
                    UpdateStatistics();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPositions()
        {
            try
            {
                using (var context = App.Context)
                {
                    _positions = context.Должности
                        .OrderBy(p => p.Наименование_должности)
                        .ToList();

                    PositionFilterComboBox.Items.Clear();

                    // Добавляем элемент "Все должности"
                    var allPositions = new Должности
                    {
                        Id_должности = 0,
                        Наименование_должности = "Все должности"
                    };
                    PositionFilterComboBox.Items.Add(allPositions);

                    foreach (var position in _positions)
                    {
                        PositionFilterComboBox.Items.Add(position);
                    }

                    PositionFilterComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки должностей: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allEmployees == null) return;

            var filtered = _allEmployees.AsQueryable();

            // Поиск по тексту
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(emp =>
                    emp.Фамилия.ToLower().Contains(searchText) ||
                    emp.Имя.ToLower().Contains(searchText) ||
                    emp.Отчество.ToLower().Contains(searchText) ||
                    emp.Должность.Наименование_должности.ToLower().Contains(searchText));
            }

            // Фильтр по должности
            if (PositionFilterComboBox.SelectedItem is Должности selectedPosition &&
                selectedPosition.Id_должности != 0)
            {
                filtered = filtered.Where(emp => emp.Id_должности == selectedPosition.Id_должности);
            }

            // Сортировка
            if (SortComboBox.SelectedItem is ComboBoxItem sortItem)
            {
                switch (sortItem.Content.ToString())
                {
                    case "По ФИО (А-Я)":
                        filtered = filtered.OrderBy(emp => emp.Фамилия)
                                          .ThenBy(emp => emp.Имя)
                                          .ThenBy(emp => emp.Отчество);
                        break;
                    case "По ФИО (Я-А)":
                        filtered = filtered.OrderByDescending(emp => emp.Фамилия)
                                          .ThenByDescending(emp => emp.Имя)
                                          .ThenByDescending(emp => emp.Отчество);
                        break;
                    case "По должности":
                        filtered = filtered.OrderBy(emp => emp.Должность.Наименование_должности)
                                          .ThenBy(emp => emp.Фамилия);
                        break;
                    case "По дате рождения":
                        filtered = filtered.OrderByDescending(emp => emp.Дата_рождения);
                        break;
                }
            }

            EmployeesDataGrid.ItemsSource = filtered.ToList();
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            if (EmployeesDataGrid.ItemsSource is IEnumerable<Сотрудник> employees)
            {
                var count = employees.Count();
                var avgAge = employees.Average(e => CalculateAge(e.Дата_рождения));

                StatisticsText.Text = $" | Показано: {count} | Средний возраст: {avgAge:F1} лет";
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Форма добавления сотрудника в разработке", "Информация");
            // NavigationService.Navigate(new AddEditEmployeePage());
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int employeeId)
            {
                var employee = _allEmployees.FirstOrDefault(emp => emp.Id_сотрудника == employeeId);
                if (employee != null)
                {
                    MessageBox.Show($"Редактирование сотрудника: {employee.Фамилия} {employee.Имя}",
                        "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
                    // NavigationService.Navigate(new AddEditEmployeePage(employeeId));
                }
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int employeeId)
            {
                var employee = _allEmployees.FirstOrDefault(emp => emp.Id_сотрудника == employeeId);
                if (employee != null)
                {
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить сотрудника?\n{employee.Фамилия} {employee.Имя} {employee.Отчество}",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            using (var context = App.Context)
                            {
                                var empToDelete = context.Сотрудники.Find(employeeId);
                                if (empToDelete != null)
                                {
                                    context.Сотрудники.Remove(empToDelete);
                                    context.SaveChanges();

                                    LoadEmployees(); // Перезагружаем данные
                                    MessageBox.Show("Сотрудник успешно удален", "Успех");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка");
                        }
                    }
                }
            }
        }

        private void ViewShifts_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int employeeId)
            {
                MessageBox.Show($"Просмотр смен сотрудника #{employeeId}", "Смены");
                // NavigationService.Navigate(new EmployeeShiftsPage(employeeId));
            }
        }

        private void CallEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string phoneNumber)
            {
                MessageBox.Show($"Имитация звонка на номер: {phoneNumber}", "Звонок");
                // Реальная логика звонка через системный вызов
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Экспорт в Excel в разработке", "Экспорт данных");
            // Реализация экспорта данных
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            PositionFilterComboBox.SelectedIndex = 0;
            SortComboBox.SelectedIndex = 0;
            EmployeesDataGrid.ItemsSource = _allEmployees;
            UpdateStatistics();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new AdminMenuPage());
        }
    }

    // Конвертер для расчета возраста
    public class DateToAgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime birthDate)
            {
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;
                if (birthDate.Date > today.AddYears(-age)) age--;
                return age.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
