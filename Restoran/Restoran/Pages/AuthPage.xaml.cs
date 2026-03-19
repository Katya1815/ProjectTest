using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RestoranApi.Models;

namespace Restoran.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TBoxLogin.Text.Trim();
            string password = TBoxPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Все поля должны быть заполнены!",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var user = App.Context.Сотрудникs
                .FirstOrDefault(u => u.Логин == login && u.Пароль == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!",
                    "Ошибка входа!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                TBoxLogin.Clear();
                TBoxPassword.Clear();
                return;
            }

            var role = App.Context.Ролиs.FirstOrDefault(r => r.IdРоли == user.IdРоли);

            if (role != null && (role.IdРоли == 5 ||
                (role.НаименованиеРоли != null && role.НаименованиеРоли.ToLower().Contains("админ"))))
            {
                App.CurrentUser = user;

                MessageBox.Show($"Добро пожаловать, {user.Имя}!\nВы авторизованы как администратор",
                    "Успешный вход!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                NavigationService?.Navigate(new EmployeeMenuPage());
            }
            else
            {
                MessageBox.Show("Недостаточно прав для входа в систему!",
                    "Ошибка доступа",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                App.CurrentUser = null;
            }

            TBoxLogin.Clear();
            TBoxPassword.Clear();
        }

        private void BtnClient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientAuthPage());
        }
    }
}