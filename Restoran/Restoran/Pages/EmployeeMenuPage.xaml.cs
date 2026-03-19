using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Restoran.Pages
{
    public partial class EmployeeMenuPage : Page
    {
        public EmployeeMenuPage()
        {
            InitializeComponent();
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage());
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UsersPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}