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
        private ObservableCollection<Клиент> Clients { get; set; }

        public UsersPage()
        {
            InitializeComponent();
            LoadClients();
        }

        // --- Загрузка клиентов из базы ---
        private void LoadClients()
        {
            try
            {
                var clients = db.Клиентs
                    .OrderBy(c => c.IdКлиента)
                    .ToList();

                Clients = new ObservableCollection<Клиент>(clients);
                UsersDataGrid.ItemsSource = Clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Добавление клиента ---
        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Window
                {
                    Title = "Добавление клиента",
                    Width = 450,
                    Height = 550,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this),
                    Background = System.Windows.Media.Brushes.White
                };

                var stackPanel = new StackPanel { Margin = new Thickness(20) };

                // Фамилия
                stackPanel.Children.Add(new TextBlock { Text = "Фамилия:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var txtLastName = new TextBox { Margin = new Thickness(0, 5, 0, 15), Height = 35 };
                stackPanel.Children.Add(txtLastName);

                // Имя
                stackPanel.Children.Add(new TextBlock { Text = "Имя:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var txtFirstName = new TextBox { Margin = new Thickness(0, 5, 0, 15), Height = 35 };
                stackPanel.Children.Add(txtFirstName);

                // Отчество
                stackPanel.Children.Add(new TextBlock { Text = "Отчество:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var txtMiddleName = new TextBox { Margin = new Thickness(0, 5, 0, 15), Height = 35 };
                stackPanel.Children.Add(txtMiddleName);

                // Дата регистрации
                stackPanel.Children.Add(new TextBlock { Text = "Дата регистрации:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var datePicker = new DatePicker { Margin = new Thickness(0, 0, 0, 15), Height = 35, SelectedDate = DateTime.Today };
                stackPanel.Children.Add(datePicker);

                // Примечания
                stackPanel.Children.Add(new TextBlock { Text = "Примечания:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var txtNotes = new TextBox { Margin = new Thickness(0, 0, 0, 20), Height = 60, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
                stackPanel.Children.Add(txtNotes);

                // Кнопки
                var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                var btnSave = new Button { Content = "Сохранить", Width = 100, Height = 35, Margin = new Thickness(5), Background = System.Windows.Media.Brushes.Orange, Foreground = System.Windows.Media.Brushes.White, BorderThickness = new Thickness(0) };
                var btnCancel = new Button { Content = "Отмена", Width = 100, Height = 35, Margin = new Thickness(5), Background = System.Windows.Media.Brushes.LightGray, Foreground = System.Windows.Media.Brushes.Black, BorderThickness = new Thickness(0) };

                btnSave.Click += (s, args) =>
                {
                    if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
                    {
                        MessageBox.Show("Заполните фамилию и имя", "Внимание");
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
                        IdРоли = 2 // клиент
                    };

                    db.Клиентs.Add(newClient);
                    db.SaveChanges();
                    Clients.Add(newClient); // сразу в ObservableCollection
                    dialog.Close();

                    MessageBox.Show("Клиент добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show($"Ошибка добавления клиента: {ex.Message}", "Ошибка");
            }
        }

        // --- Редактирование клиента ---
        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = UsersDataGrid.SelectedItem as Клиент;
            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dialog = new Window
                {
                    Title = "Редактирование клиента",
                    Width = 450,
                    Height = 550,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this),
                    Background = System.Windows.Media.Brushes.White
                };

                var stackPanel = new StackPanel { Margin = new Thickness(20) };

                // Фамилия
                stackPanel.Children.Add(new TextBlock { Text = "Фамилия:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var txtLastName = new TextBox { Text = selectedClient.Фамилия, Margin = new Thickness(0, 5, 0, 15), Height = 35 };
                stackPanel.Children.Add(txtLastName);

                // Имя
                stackPanel.Children.Add(new TextBlock { Text = "Имя:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var txtFirstName = new TextBox { Text = selectedClient.Имя, Margin = new Thickness(0, 5, 0, 15), Height = 35 };
                stackPanel.Children.Add(txtFirstName);

                // Отчество
                stackPanel.Children.Add(new TextBlock { Text = "Отчество:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var txtMiddleName = new TextBox { Text = selectedClient.Отчество, Margin = new Thickness(0, 5, 0, 15), Height = 35 };
                stackPanel.Children.Add(txtMiddleName);

                // Дата регистрации
                stackPanel.Children.Add(new TextBlock { Text = "Дата регистрации:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var datePicker = new DatePicker { Margin = new Thickness(0, 0, 0, 15), Height = 35 };
                datePicker.SelectedDate = selectedClient.ДатаРегистрацииВСистеме.ToDateTime(TimeOnly.MinValue);
                stackPanel.Children.Add(datePicker);

                // Примечания
                stackPanel.Children.Add(new TextBlock { Text = "Примечания:", FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Orange });
                var txtNotes = new TextBox { Text = selectedClient.Примечания, Margin = new Thickness(0, 0, 0, 20), Height = 60, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
                stackPanel.Children.Add(txtNotes);

                // Кнопки
                var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                var btnSave = new Button { Content = "Сохранить", Width = 100, Height = 35, Margin = new Thickness(5), Background = System.Windows.Media.Brushes.Orange, Foreground = System.Windows.Media.Brushes.White, BorderThickness = new Thickness(0) };
                var btnCancel = new Button { Content = "Отмена", Width = 100, Height = 35, Margin = new Thickness(5), Background = System.Windows.Media.Brushes.LightGray, Foreground = System.Windows.Media.Brushes.Black, BorderThickness = new Thickness(0) };

                btnSave.Click += (s, args) =>
                {
                    if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
                    {
                        MessageBox.Show("Заполните фамилию и имя", "Внимание");
                        return;
                    }

                    selectedClient.Фамилия = txtLastName.Text;
                    selectedClient.Имя = txtFirstName.Text;
                    selectedClient.Отчество = txtMiddleName.Text;
                    selectedClient.ДатаРегистрацииВСистеме = datePicker.SelectedDate.HasValue
                        ? DateOnly.FromDateTime(datePicker.SelectedDate.Value)
                        : selectedClient.ДатаРегистрацииВСистеме;
                    selectedClient.Примечания = txtNotes.Text;

                    db.SaveChanges();
                    UsersDataGrid.Items.Refresh();
                    dialog.Close();

                    MessageBox.Show("Клиент обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show($"Ошибка редактирования клиента: {ex.Message}", "Ошибка");
            }
        }

        // --- Удаление клиента ---
        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = UsersDataGrid.SelectedItem as Клиент;
            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить клиента {selectedClient.Фамилия} {selectedClient.Имя}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                db.Клиентs.Remove(selectedClient);
                db.SaveChanges();
                Clients.Remove(selectedClient);
            }
        }

        // --- Обновление списка ---
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }
    }
}