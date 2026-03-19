using Microsoft.EntityFrameworkCore;
using RestoranApi.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restoran.Pages
{
    public partial class EmployeesPage : Page
    {
        private RestoranIs32Context db = new RestoranIs32Context();
        private ObservableCollection<Сотрудник> Employees { get; set; }

        public EmployeesPage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = db.Сотрудникs
                    .Include(s => s.IdДолжностиNavigation)
                    .Include(s => s.IdРолиNavigation)
                    .ToList();

                Employees = new ObservableCollection<Сотрудник>(employees);
                EmployeesDataGrid.ItemsSource = Employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создаем окно для ввода данных
                var dialog = new Window
                {
                    Title = "Добавление сотрудника",
                    Width = 400,
                    Height = 650,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this),
                    Background = System.Windows.Media.Brushes.White
                };

                var scrollViewer = new ScrollViewer();
                var stackPanel = new StackPanel { Margin = new Thickness(20) };

                // Фамилия
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Фамилия:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });
                var txtLastName = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Height = 30,
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1)
                };
                stackPanel.Children.Add(txtLastName);

                // Имя
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Имя:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });
                var txtFirstName = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Height = 30,
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1)
                };
                stackPanel.Children.Add(txtFirstName);

                // Отчество
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Отчество:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });
                var txtMiddleName = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Height = 30,
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1)
                };
                stackPanel.Children.Add(txtMiddleName);

                // Логин
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Логин:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });
                var txtLogin = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Height = 30,
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1)
                };
                stackPanel.Children.Add(txtLogin);

                // Пароль
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Пароль:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });
                var txtPassword = new PasswordBox
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Height = 30,
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1)
                };
                stackPanel.Children.Add(txtPassword);

                // Телефон
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Телефон:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });
                var txtPhone = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Height = 30,
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1)
                };
                stackPanel.Children.Add(txtPhone);

                // ДАТА РОЖДЕНИЯ
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Дата рождения:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });

                // Создаем StackPanel для горизонтального расположения элементов даты
                var datePanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };

                // ИСПРАВЛЕНО: Объявляем все переменные ДО их использования
                TextBox txtDay = null;
                TextBox txtMonth = null;
                TextBox txtYear = null;

                // День
                datePanel.Children.Add(new TextBlock { Text = "День:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 5, 0) });
                txtDay = new TextBox
                {
                    Width = 50,
                    Height = 30,
                    Margin = new Thickness(0, 0, 5, 0),
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1),
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                datePanel.Children.Add(txtDay);

                // Месяц
                datePanel.Children.Add(new TextBlock { Text = "Месяц:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 5, 0) });
                txtMonth = new TextBox
                {
                    Width = 50,
                    Height = 30,
                    Margin = new Thickness(0, 0, 5, 0),
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1),
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                datePanel.Children.Add(txtMonth);

                // Год
                datePanel.Children.Add(new TextBlock { Text = "Год:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 5, 0) });
                txtYear = new TextBox
                {
                    Width = 70,
                    Height = 30,
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1),
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                datePanel.Children.Add(txtYear);

                // ИСПРАВЛЕНО: Теперь можно использовать переменные, так как они уже объявлены
                txtDay.TextChanged += (s, args) =>
                {
                    if (txtDay.Text.Length == 2 && txtMonth != null)
                    {
                        txtMonth.Focus();
                    }
                };

                txtMonth.TextChanged += (s, args) =>
                {
                    if (txtMonth.Text.Length == 2 && txtYear != null)
                    {
                        txtYear.Focus();
                    }
                };

                stackPanel.Children.Add(datePanel);

                // Выбор должности
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Должность:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });
                var cmbPosition = new ComboBox
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Height = 30,
                    DisplayMemberPath = "НаименованиеДолжности",
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1)
                };
                cmbPosition.ItemsSource = db.Должностиs.ToList();
                stackPanel.Children.Add(cmbPosition);

                // Выбор роли
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Роль:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 2)
                });
                var cmbRole = new ComboBox
                {
                    Margin = new Thickness(0, 0, 0, 20),
                    Height = 30,
                    DisplayMemberPath = "НаименованиеРоли",
                    BorderBrush = System.Windows.Media.Brushes.Orange,
                    BorderThickness = new Thickness(1)
                };
                cmbRole.ItemsSource = db.Ролиs.ToList();
                stackPanel.Children.Add(cmbRole);

                // Кнопки
                var btnPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var btnSave = new Button
                {
                    Content = "Сохранить",
                    Width = 100,
                    Height = 35,
                    Margin = new Thickness(5),
                    Background = System.Windows.Media.Brushes.Orange,
                    Foreground = System.Windows.Media.Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                var btnCancel = new Button
                {
                    Content = "Отмена",
                    Width = 100,
                    Height = 35,
                    Margin = new Thickness(5),
                    Background = System.Windows.Media.Brushes.LightGray,
                    Foreground = System.Windows.Media.Brushes.Black,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                btnSave.Click += (s, args) =>
                {
                    // Проверка заполнения обязательных полей
                    if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                        string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                        string.IsNullOrWhiteSpace(txtLogin.Text) ||
                        string.IsNullOrWhiteSpace(txtPassword.Password) ||
                        cmbPosition.SelectedItem == null ||
                        cmbRole.SelectedItem == null)
                    {
                        MessageBox.Show("Заполните все обязательные поля (Фамилия, Имя, Логин, Пароль, Должность, Роль)",
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Обработка даты рождения
                    DateOnly? birthDate = null;
                    if (!string.IsNullOrWhiteSpace(txtDay.Text) &&
                        !string.IsNullOrWhiteSpace(txtMonth.Text) &&
                        !string.IsNullOrWhiteSpace(txtYear.Text))
                    {
                        try
                        {
                            int day = int.Parse(txtDay.Text);
                            int month = int.Parse(txtMonth.Text);
                            int year = int.Parse(txtYear.Text);

                            if (day > 0 && day <= 31 && month > 0 && month <= 12 && year > 1900 && year <= DateTime.Now.Year)
                            {
                                birthDate = new DateOnly(year, month, day);
                            }
                            else
                            {
                                MessageBox.Show("Некорректная дата рождения", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Введите корректную дату рождения", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Создание нового сотрудника
                    var newEmployee = new Сотрудник
                    {
                        Фамилия = txtLastName.Text,
                        Имя = txtFirstName.Text,
                        Отчество = txtMiddleName.Text,
                        Логин = txtLogin.Text,
                        Пароль = txtPassword.Password,
                        НомерТелефона = txtPhone.Text,
                        ДатаРождения = birthDate,
                        IdДолжности = ((Должности)cmbPosition.SelectedItem).IdДолжности,
                        IdРоли = ((Роли)cmbRole.SelectedItem).IdРоли
                    };

                    db.Сотрудникs.Add(newEmployee);
                    db.SaveChanges();

                    dialog.Close();
                    LoadEmployees();

                    MessageBox.Show($"Сотрудник {newEmployee.Фамилия} {newEmployee.Имя} добавлен",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                };

                btnCancel.Click += (s, args) => dialog.Close();

                // Скругляем углы кнопок
                var buttonStyle = new Style(typeof(Border));
                buttonStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(5)));

                btnSave.Resources.Add(typeof(Border), buttonStyle);
                btnCancel.Resources.Add(typeof(Border), buttonStyle);

                btnPanel.Children.Add(btnSave);
                btnPanel.Children.Add(btnCancel);
                stackPanel.Children.Add(btnPanel);

                scrollViewer.Content = stackPanel;
                dialog.Content = scrollViewer;
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeesDataGrid.SelectedItem as Сотрудник;

            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Редактирование сотрудника {selectedEmployee.Фамилия} {selectedEmployee.Имя}\nФункция в разработке",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeesDataGrid.SelectedItem as Сотрудник;

            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить сотрудника {selectedEmployee.Фамилия} {selectedEmployee.Имя}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    db.Сотрудникs.Remove(selectedEmployee);
                    db.SaveChanges();
                    LoadEmployees();

                    MessageBox.Show("Сотрудник удален", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }
    }
}