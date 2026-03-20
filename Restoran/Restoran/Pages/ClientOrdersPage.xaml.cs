using RestoranApi.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Restoran.Pages
{
    public class OrderViewModel
    {
        public int IdЗаказа { get; set; }
        public string Статус { get; set; }
        public DateTime ДатаПринятияЗаказа { get; set; } // DateOnly -> DateTime
        public TimeSpan ВремяПринятияЗаказа { get; set; } // TimeOnly -> TimeSpan
        public decimal Сумма { get; set; }
    }

    public partial class ClientOrdersPage : Page
    {
        private RestoranIs32Context db = new RestoranIs32Context();
        private ObservableCollection<OrderViewModel> ClientOrders { get; set; }

        public ClientOrdersPage()
        {
            InitializeComponent();
            LoadClientOrders();
        }

        private void LoadClientOrders()
        {
            try
            {
                int currentClientId = App.CurrentClient?.IdКлиента ?? 1;

                var orders = db.Заказs
                    .Where(o => o.Заказчик == currentClientId)
                    .OrderByDescending(o => o.ДатаПринятияЗаказа)
                    .ThenByDescending(o => o.ВремяПринятияЗаказа)
                    .Select(o => new OrderViewModel
                    {
                        IdЗаказа = o.IdЗаказа,
                        Статус = o.Статус,
                        ДатаПринятияЗаказа = o.ДатаПринятияЗаказа.ToDateTime(TimeOnly.MinValue), // DateOnly -> DateTime
                        ВремяПринятияЗаказа = o.ВремяПринятияЗаказа.ToTimeSpan(), // TimeOnly -> TimeSpan
                        Сумма = o.Сумма
                    })
                    .ToList();

                ClientOrders = new ObservableCollection<OrderViewModel>(orders);
                OrdersItemsControl.ItemsSource = ClientOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshOrders()
        {
            LoadClientOrders();
        }

        private void BtnMakeOrder_Click(object sender, RoutedEventArgs e)
        {
            var makeOrderPage = new MakeOrderPage();
            makeOrderPage.OrderCreated += OnOrderCreated;
            NavigationService?.Navigate(makeOrderPage);
        }

        private void OnOrderCreated()
        {
            LoadClientOrders();
            MessageBox.Show("Ваш заказ создан! Список обновлен.", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnRepeatOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                var originalOrder = db.Заказs.FirstOrDefault(o => o.IdЗаказа == orderId);
                if (originalOrder == null) return;

                var makeOrderPage = new MakeOrderPage();
                makeOrderPage.OrderCreated += OnOrderCreated;
                NavigationService?.Navigate(makeOrderPage);
            }
        }

        private void BtnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                var order = db.Заказs.FirstOrDefault(o => o.IdЗаказа == orderId);
                if (order == null) return;

                if (order.Статус == "Готов" || order.Статус == "Доставлен")
                {
                    MessageBox.Show("Этот заказ уже нельзя отменить");
                    return;
                }

                var result = MessageBox.Show($"Отменить заказ №{orderId}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    order.Статус = "Отменен";
                    db.SaveChanges();
                    LoadClientOrders();
                    MessageBox.Show("Заказ отменен", "Успех");
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }

    // Расширения для конверсии
    public static class TimeOnlyExtensions
    {
        public static TimeSpan ToTimeSpan(this TimeOnly time)
        {
            return new TimeSpan(time.Hour, time.Minute, time.Second);
        }
    }
}