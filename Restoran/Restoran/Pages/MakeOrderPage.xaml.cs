using RestoranApi.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restoran.Pages
{
    public partial class MakeOrderPage : Page
    {
        private RestoranIs32Context db = new RestoranIs32Context();
        private ObservableCollection<Блюдо> AllDishes { get; set; }
        private ObservableCollection<Блюдо> FilteredDishes { get; set; }
        private ObservableCollection<CartItem> Cart { get; set; } = new ObservableCollection<CartItem>();

        // Событие для уведомления о создании заказа
        public event Action OrderCreated;

        public class CartItem
        {
            public int DishId { get; set; }
            public string DishName { get; set; }
            public decimal Price { get; set; }
        }

        public MakeOrderPage()
        {
            InitializeComponent();
            LoadMenu();
            CartListBox.ItemsSource = Cart;
            UpdateTotalPrice();
        }

        private void LoadMenu()
        {
            try
            {
                var dishes = db.Блюдоs.ToList();
                AllDishes = new ObservableCollection<Блюдо>(dishes);
                FilteredDishes = new ObservableCollection<Блюдо>(dishes);
                MenuItemsControl.ItemsSource = FilteredDishes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки меню: {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllDishes == null) return;

            string searchText = SearchBox.Text.ToLower();
            FilteredDishes.Clear();

            var filtered = AllDishes.Where(d =>
                (d.Наименование?.ToLower().Contains(searchText) ?? false) ||
                (d.ОписаниеБлюда?.ToLower().Contains(searchText) ?? false) ||
                (d.КатегорияБлюда?.ToLower().Contains(searchText) ?? false)
            );

            foreach (var dish in filtered)
                FilteredDishes.Add(dish);
        }

        private void BtnAddDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            if (!int.TryParse(button.Tag?.ToString(), out int dishId)) return;

            var dish = AllDishes.FirstOrDefault(d => d.IdБлюда == dishId);
            if (dish == null) return;

            Cart.Add(new CartItem
            {
                DishId = dish.IdБлюда,
                DishName = dish.Наименование,
                Price = dish.Цена
            });

            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            decimal total = Cart.Sum(item => item.Price);
            TotalPriceText.Text = $"{total:N2} ₽";
        }

        private void BtnClearCart_Click(object sender, RoutedEventArgs e)
        {
            Cart.Clear();
            UpdateTotalPrice();
        }

        private void BtnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Cart.Count == 0)
                {
                    MessageBox.Show("Добавьте хотя бы одно блюдо в заказ");
                    return;
                }

                var selectedTable = TableComboBox.SelectedItem as ComboBoxItem;
                if (selectedTable == null)
                {
                    MessageBox.Show("Выберите столик");
                    return;
                }

                int tableNumber = int.Parse(selectedTable.Tag.ToString());

                // Получаем ID текущего клиента из App.CurrentClient
                int currentClientId = App.CurrentClient?.IdКлиента ?? 1;

                var client = db.Клиентs.FirstOrDefault(c => c.IdКлиента == currentClientId);
                if (client == null)
                {
                    MessageBox.Show("Клиент не найден");
                    return;
                }

                // автоматический выбор первого официанта
                var waiter = db.Сотрудникs.FirstOrDefault();
                if (waiter == null)
                {
                    MessageBox.Show("В базе нет ни одного сотрудника");
                    return;
                }

                var newOrder = new Заказ
                {
                    НомерСтолика = tableNumber,
                    Заказчик = currentClientId,
                    Официант = waiter.IdСотрудника,
                    ДатаПринятияЗаказа = DateOnly.FromDateTime(DateTime.Now),
                    ВремяПринятияЗаказа = TimeOnly.FromDateTime(DateTime.Now),
                    Статус = "Новый",
                    Сумма = Cart.Sum(c => c.Price),
                    Оплачено = false
                };

                db.Заказs.Add(newOrder);
                db.SaveChanges();

                // ВЫЗЫВАЕМ СОБЫТИЕ для обновления всех подписанных страниц
                OrderCreated?.Invoke();

                MessageBox.Show($"Заказ №{newOrder.IdЗаказа} оформлен!\nСумма: {newOrder.Сумма:N2} ₽");

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}\n\n{ex.InnerException?.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}