
using RestoranApi.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restoran.Pages
{
    public partial class OrdersPage : Page
    {
        RestoranIs32Context db = new RestoranIs32Context();

        public OrdersPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            OrdersGrid.ItemsSource = db.Заказs.ToList();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var order = OrdersGrid.SelectedItem as Заказ;

            if (order == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            db.Заказs.Remove(order);
            db.SaveChanges();

            LoadOrders();
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            var order = OrdersGrid.SelectedItem as Заказ;

            if (order == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            order.Статус = "Готов";
            db.SaveChanges();

            LoadOrders();
        }
    }
}