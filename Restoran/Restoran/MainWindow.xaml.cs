using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restoran
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MaineFrame.Navigate(new Pages.AuthPage());
        }
        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            Page page = MaineFrame.Content as Page;
            if (page.Title == "Авторизация")
            {
                BtnBack.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnBack.Visibility = Visibility.Visible;
            }
        }
        private void BtnBack_Back(object sender, RoutedEventArgs e)
        {
            MaineFrame.GoBack();
        }
       
    }
}