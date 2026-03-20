using RestoranApi.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restoran.Pages
{
    public partial class OrdersPage : Page
    {
        private RestoranIs32Context db = new RestoranIs32Context();

        // Используем ObservableCollection для автоматического обновления DataGrid
        public ObservableCollection<ЗаказViewModel> Orders { get; set; } = new ObservableCollection<ЗаказViewModel>();

        public OrdersPage()
        {
            InitializeComponent();
            OrdersGrid.ItemsSource = Orders;
            LoadOrders();
        }

        // Метод загрузки заказов из БД
        public void LoadOrders()
        {
            Orders.Clear();

            var allOrders = db.Заказs
                              .ToList()
                              .Select(o => new ЗаказViewModel
                              {
                                  Id = o.IdЗаказа,
                                  ClientName = db.Клиентs.FirstOrDefault(c => c.IdКлиента == o.Заказчик)?.Фамилия ?? "Неизвестно",
                                  Dish = string.Join(", ", db.Блюдоs
                                                             .Where(b => b.IdБлюда == o.IdЗаказа)
                                                             .Select(b => b.Наименование)),
                                  Quantity = 1, // пока без детализации, можно расширить
                                  Status = o.Статус
                              });

            foreach (var order in allOrders)
                Orders.Add(order);
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrderVM = OrdersGrid.SelectedItem as ЗаказViewModel;
            if (selectedOrderVM == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            var order = db.Заказs.FirstOrDefault(o => o.IdЗаказа == selectedOrderVM.Id);
            if (order == null) return;

            db.Заказs.Remove(order);
            db.SaveChanges();

            LoadOrders();
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrderVM = OrdersGrid.SelectedItem as ЗаказViewModel;
            if (selectedOrderVM == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            var order = db.Заказs.FirstOrDefault(o => o.IdЗаказа == selectedOrderVM.Id);
            if (order == null) return;

            order.Статус = "Готов";
            db.SaveChanges();

            LoadOrders();
        }

        // Метод для обновления списка из других страниц (например, MakeOrderPage)
        public void RefreshOrders()
        {
            LoadOrders();
        }
    }

    // Вспомогательный ViewModel для отображения заказов
    public class ЗаказViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Dish { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}