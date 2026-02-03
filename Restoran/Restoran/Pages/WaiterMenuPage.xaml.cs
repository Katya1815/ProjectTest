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
    /// Логика взаимодействия для WaiterMenuPage.xaml
    /// </summary>
    public partial class WaiterMenuPage : Page
    {
        public WaiterMenuPage()
        {
            InitializeComponent();
        }

        private void ViewOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage());
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderCreatePage());
        }

        private void ViewTables_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Страница столиков в разработке", "Информация");
            // NavigationService.Navigate(new TablesPage());
        }

        private void ViewMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DishesPage());
        }

        private void ViewClients_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Страница клиентов в разработке", "Информация");
            // NavigationService.Navigate(new ClientsPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти из системы?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new AuthPage());
            }
        }
    }
}
