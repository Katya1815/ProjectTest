using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public int Count; //Переменная для подсчета неудачных попыток входа
        public AuthPage()
        {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (TBoxLogin.Text != "" && TBoxPassword.Password != "")
            {
                Count = 0;//Сброс подсчета неверных попыток после успешной авторизации
                if (user.Block == false)// Проверка на блокировку учетной записи
                {
                    TBoxLogin.Text = "";
                    if (user.Role == 1)
                    {
                        MessageBox.Show("Вы успешно авторизовались как Администратор", "Успешный вход!", MessageBoxButton.OK, MessageBoxImage.Information);
                        App.CurrentUser = user;
                        if (user.FirstAuth == false)//Проверка на прохождение первой авторизации
                        {
                            user.FirstAuth = true;
                            DataContext = user;
                            App.Context.SaveChanges();
                            NavigationService.Navigate(new ChangePasswordPage());
                        }
                        else
                            NavigationService.Navigate(new AdminMenuPage());
                    }
                    else if (user.Role == 2)
                    {
                        MessageBox.Show("Вы успешно авторизовались как Пользователь", "Успешный вход!", MessageBoxButton.OK, MessageBoxImage.Information);
                        App.CurrentUser = user;
                        if (user.FirstAuth == false)//Проверка на прохождение первой авторизации
                        {
                            user.FirstAuth = true;
                            DataContext = user;
                            App.Context.SaveChanges();
                            NavigationService.Navigate(new ChangePasswordPage());
                        }
                        else
                            NavigationService.Navigate(new AdminMenuPage());
                    }
                    else if (user.Role == 3)
                    {
                        MessageBox.Show("Недостаточно прав для использолвания системы!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Вы заблокированы!", "Обратитесь к администратору!", MessageBoxButton.OK, MessageBoxImage.Error);
                    TBoxLogin.Text = "";
                    TBoxPassword.Password = "";
                }
            }
            else
            {
                MessageBox.Show("пожалуйста проверьте ещё раз введенные данные!", "Вы ввели неверный логин или пароль!", MessageBoxButton.OK, MessageBoxImage.Error);
                Count++;
                TBoxLogin.Text = "";
                TBoxPassword.Password = "";
                if (App.Context.Users.FirstOrDefault(p => p.Login == TBoxLogin.Text) is User userBlock)//Блокировка записи после неудачной авторизации
                {
                    if (Count == 3)
                    {
                        userBlock.Block = true;
                        DataContext = userBlock;
                        App.Context.SaveChanges();
                        Count = 0;
                    }
                }

                else
                {
                    MessageBox.Show("Все поля должны быть заполнены!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
