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
    /// Логика взаимодействия для OrderCreatePage.xaml
    /// </summary>
    public partial class OrderCreatePage : Page
    {
        private List<Блюдо> _availableDishes;
        private List<OrderItem> _selectedDishes = new List<OrderItem>();
        private List<Столик> _tables;
        private List<Клиент> _clients;

        public class OrderItem
        {
            public int Id_блюда { get; set; }
            public string Наименование { get; set; }
            public decimal Цена { get; set; }
            public int Количество { get; set; } = 1;
            public decimal Сумма => Цена * Количество;
        }

        public OrderCreatePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = App.Context)
                {
                    // Загрузка доступных блюд
                    _availableDishes = context.Блюда
                        .Where(d => d.Меню.Any(m => m.Доступно_для_заказа))
                        .ToList();

                    AvailableDishesGrid.ItemsSource = _availableDishes;

                    // Загрузка столиков
                    _tables = context.Столики
                        .Where(t => t.Статус == "свободен")
                        .ToList();

                    TableComboBox.ItemsSource = _tables;
                    TableComboBox.DisplayMemberPath = "Номер_столика";
                    TableComboBox.SelectedValuePath = "Id_столика";

                    // Загрузка клиентов
                    _clients = context.Клиенты.ToList();
                    ClientComboBox.ItemsSource = _clients;
                    ClientComboBox.DisplayMemberPath = "ФИО";
                    ClientComboBox.SelectedValuePath = "Id_клиента";

                    UpdateTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void AddDishToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int dishId)
            {
                AddDishToOrder(dishId);
            }
            else if (AvailableDishesGrid.SelectedItem is Блюдо selectedDish)
            {
                AddDishToOrder(selectedDish.Id_блюда);
            }
        }

        private void AddDishToOrder(int dishId)
        {
            var dish = _availableDishes.FirstOrDefault(d => d.Id_блюда == dishId);
            if (dish == null) return;

            var existingItem = _selectedDishes.FirstOrDefault(i => i.Id_блюда == dishId);
            if (existingItem != null)
            {
                existingItem.Количество++;
            }
            else
            {
                _selectedDishes.Add(new OrderItem
                {
                    Id_блюда = dish.Id_блюда,
                    Наименование = dish.Наименование,
                    Цена = dish.Цена,
                    Количество = 1
                });
            }

            SelectedDishesGrid.ItemsSource = null;
            SelectedDishesGrid.ItemsSource = _selectedDishes;
            UpdateTotal();
        }

        private void RemoveDishFromOrder_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDishesGrid.SelectedItem is OrderItem selectedItem)
            {
                RemoveDish(selectedItem.Id_блюда);
            }
        }

        private void RemoveDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int dishId)
            {
                RemoveDish(dishId);
            }
        }

        private void RemoveDish(int dishId)
        {
            var item = _selectedDishes.FirstOrDefault(i => i.Id_блюда == dishId);
            if (item != null)
            {
                if (item.Количество > 1)
                {
                    item.Количество--;
                }
                else
                {
                    _selectedDishes.Remove(item);
                }

                SelectedDishesGrid.ItemsSource = null;
                SelectedDishesGrid.ItemsSource = _selectedDishes;
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            decimal total = _selectedDishes.Sum(item => item.Сумма);
            TotalAmountText.Text = $"{total:C}";
        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (TableComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите столик!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedDishes.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одно блюдо в заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = App.Context)
                {
                    // Создание нового заказа
                    var newOrder = new Заказ
                    {
                        Номер_столика = ((Столик)TableComboBox.SelectedItem).Номер_столика,
                        Id_клиента = ClientComboBox.SelectedValue as int?,
                        Id_сотрудника = GetCurrentEmployeeId(), // Получаем ID текущего сотрудника
                        Дата_принятия_заказа = DateTime.Now.Date,
                        Время_принятия_заказа = DateTime.Now.TimeOfDay,
                        Описание_заказа = NoteTextBox.Text,
                        Статус = "Новый",
                        Сумма = _selectedDishes.Sum(item => item.Сумма),
                        Оплачено = false
                    };

                    context.Заказы.Add(newOrder);
                    context.SaveChanges();

                    // Добавление состава заказа
                    foreach (var item in _selectedDishes)
                    {
                        var orderDetail = new Состав_заказа
                        {
                            Id_заказа = newOrder.Id_заказа,
                            Id_блюда = item.Id_блюда,
                            Количество = item.Количество,
                            Цена_на_момент_заказа = item.Цена
                        };
                        context.Состав_заказа.Add(orderDetail);
                    }

                    // Обновление статуса столика
                    var table = context.Столики.First(t => t.Id_столика == ((Столик)TableComboBox.SelectedItem).Id_столика);
                    table.Статус = "занят";

                    context.SaveChanges();

                    MessageBox.Show($"Заказ #{newOrder.Id_заказа} успешно создан!\nСумма: {newOrder.Сумма:C}",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService.Navigate(new OrdersPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения заказа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetCurrentEmployeeId()
        {
            // Здесь должна быть логика получения ID текущего авторизованного сотрудника
            // Временно возвращаем 1 (первого сотрудника)
            return 1;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отменить создание заказа? Все данные будут потеряны.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService.GoBack();
            }
        }
    }
}
