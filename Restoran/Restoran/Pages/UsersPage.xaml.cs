using RestoranApi.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restoran.Pages
{
    public partial class UsersPage : Page
    {
        private RestoranIs32Context db = new RestoranIs32Context();
        private ObservableCollection<Клиент> Clients { get; set; } = new ObservableCollection<Клиент>();

        public UsersPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                // Используем правильный DbSet из контекста
                var clientsFromDb = db.Клиентs.ToList(); // или db.Клиенты, если так в вашем контексте

                Clients = new ObservableCollection<Клиент>(clientsFromDb);
                UsersDataGrid.ItemsSource = Clients;

                if (Clients.Count == 0)
                {
                    MessageBox.Show("В базе нет клиентов", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Window
                {
                    Title = "Добавление клиента",
                    Width = 400,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this)
                };

                var stackPanel = new StackPanel { Margin = new Thickness(20) };

                // Фамилия
                stackPanel.Children.Add(new TextBlock { Text = "Фамилия:", FontWeight = FontWeights.Bold });
                var txtLastName = new TextBox { Margin = new Thickness(0, 5, 0, 10), Height = 30 };
                stackPanel.Children.Add(txtLastName);

                // Имя
                stackPanel.Children.Add(new TextBlock { Text = "Имя:", FontWeight = FontWeights.Bold });
                var txtFirstName = new TextBox { Margin = new Thickness(0, 5, 0, 10), Height = 30 };
                stackPanel.Children.Add(txtFirstName);

                // Отчество
                stackPanel.Children.Add(new TextBlock { Text = "Отчество:", FontWeight = FontWeights.Bold });
                var txtMiddleName = new TextBox { Margin = new Thickness(0, 5, 0, 10), Height = 30 };
                stackPanel.Children.Add(txtMiddleName);

                // Дата регистрации
                stackPanel.Children.Add(new TextBlock { Text = "Дата регистрации:", FontWeight = FontWeights.Bold });
                var datePicker = new DatePicker { Margin = new Thickness(0, 5, 0, 10), Height = 30, SelectedDate = DateTime.Today };
                stackPanel.Children.Add(datePicker);

                // Примечания
                stackPanel.Children.Add(new TextBlock { Text = "Примечания:", FontWeight = FontWeights.Bold });
                var txtNotes = new TextBox { Margin = new Thickness(0, 5, 0, 20), Height = 60, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
                stackPanel.Children.Add(txtNotes);

                // Кнопки
                var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

                var btnSave = new Button
                {
                    Content = "Сохранить",
                    Width = 100,
                    Height = 35,
                    Margin = new Thickness(5),
                    Background = System.Windows.Media.Brushes.Orange,
                    Foreground = System.Windows.Media.Brushes.White,
                    BorderThickness = new Thickness(0)
                };

                var btnCancel = new Button
                {
                    Content = "Отмена",
                    Width = 100,
                    Height = 35,
                    Margin = new Thickness(5),
                    Background = System.Windows.Media.Brushes.LightGray,
                    Foreground = System.Windows.Media.Brushes.Black,
                    BorderThickness = new Thickness(0)
                };

                btnSave.Click += (s, args) =>
                {
                    if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
                    {
                        MessageBox.Show("Заполните фамилию и имя");
                        return;
                    }

                    var newClient = new Клиент
                    {
                        Фамилия = txtLastName.Text,
                        Имя = txtFirstName.Text,
                        Отчество = txtMiddleName.Text,
                        ДатаРегистрацииВСистеме = datePicker.SelectedDate.HasValue
                            ? DateOnly.FromDateTime(datePicker.SelectedDate.Value)
                            : DateOnly.FromDateTime(DateTime.Today),
                        Примечания = txtNotes.Text,
                        IdРоли = 2
                    };

                    db.Клиентs.Add(newClient); // DbSet Клиент
                    db.SaveChanges();

                    Clients.Add(newClient); // Обновляем ObservableCollection сразу
                    dialog.Close();
                    MessageBox.Show("Клиент добавлен", "Успех");
                };

                btnCancel.Click += (s, args) => dialog.Close();

                btnPanel.Children.Add(btnSave);
                btnPanel.Children.Add(btnCancel);
                stackPanel.Children.Add(btnPanel);

                dialog.Content = stackPanel;
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            var selected = UsersDataGrid.SelectedItem as Клиент;
            if (selected == null)
            {
                MessageBox.Show("Выберите клиента");
                return;
            }

            var dialog = new Window
            {
                Title = "Редактирование клиента",
                Width = 400,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            stackPanel.Children.Add(new TextBlock { Text = "Фамилия:", FontWeight = FontWeights.Bold });
            var txtLastName = new TextBox { Text = selected.Фамилия, Margin = new Thickness(0, 5, 0, 10), Height = 30 };
            stackPanel.Children.Add(txtLastName);

            stackPanel.Children.Add(new TextBlock { Text = "Имя:", FontWeight = FontWeights.Bold });
            var txtFirstName = new TextBox { Text = selected.Имя, Margin = new Thickness(0, 5, 0, 10), Height = 30 };
            stackPanel.Children.Add(txtFirstName);

            stackPanel.Children.Add(new TextBlock { Text = "Отчество:", FontWeight = FontWeights.Bold });
            var txtMiddleName = new TextBox { Text = selected.Отчество, Margin = new Thickness(0, 5, 0, 10), Height = 30 };
            stackPanel.Children.Add(txtMiddleName);

            stackPanel.Children.Add(new TextBlock { Text = "Дата регистрации:", FontWeight = FontWeights.Bold });
            var datePicker = new DatePicker
            {
                SelectedDate = selected.ДатаРегистрацииВСистеме.ToDateTime(TimeOnly.MinValue),
                Margin = new Thickness(0, 5, 0, 10),
                Height = 30
            };
            stackPanel.Children.Add(datePicker);

            stackPanel.Children.Add(new TextBlock { Text = "Примечания:", FontWeight = FontWeights.Bold });
            var txtNotes = new TextBox { Text = selected.Примечания, Margin = new Thickness(0, 5, 0, 20), Height = 60, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
            stackPanel.Children.Add(txtNotes);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

            var btnSave = new Button
            {
                Content = "Сохранить",
                Width = 100,
                Height = 35,
                Margin = new Thickness(5),
                Background = System.Windows.Media.Brushes.Orange,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0)
            };

            var btnCancel = new Button
            {
                Content = "Отмена",
                Width = 100,
                Height = 35,
                Margin = new Thickness(5),
                Background = System.Windows.Media.Brushes.LightGray,
                Foreground = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(0)
            };

            btnSave.Click += (s, args) =>
            {
                if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
                {
                    MessageBox.Show("Заполните фамилию и имя");
                    return;
                }

                selected.Фамилия = txtLastName.Text;
                selected.Имя = txtFirstName.Text;
                selected.Отчество = txtMiddleName.Text;
                selected.ДатаРегистрацииВСистеме = datePicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(datePicker.SelectedDate.Value)
                    : DateOnly.FromDateTime(DateTime.Today);
                selected.Примечания = txtNotes.Text;

                db.SaveChanges();
                UsersDataGrid.Items.Refresh(); // Обновляем DataGrid
                dialog.Close();
                MessageBox.Show("Клиент обновлен", "Успех");
            };

            btnCancel.Click += (s, args) => dialog.Close();

            btnPanel.Children.Add(btnSave);
            btnPanel.Children.Add(btnCancel);
            stackPanel.Children.Add(btnPanel);

            dialog.Content = stackPanel;
            dialog.ShowDialog();
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            var selected = UsersDataGrid.SelectedItem as Клиент;
            if (selected == null)
            {
                MessageBox.Show("Выберите клиента");
                return;
            }

            if (MessageBox.Show($"Удалить {selected.Фамилия} {selected.Имя}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                db.Клиентs.Remove(selected);
                db.SaveChanges();
                Clients.Remove(selected); // Удаляем из коллекции
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }
    }
}