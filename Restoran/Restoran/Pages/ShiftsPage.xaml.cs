using RestoranApi.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restoran.Pages
{
    public partial class ShiftsPage : Page
    {
        private RestoranIs32Context db = new RestoranIs32Context(); 
        private ObservableCollection<ShiftViewModel> Shifts { get; set; }

        public class ShiftViewModel
        {
            public int id_смены { get; set; }
            public string EmployeeName { get; set; }
            public string Position { get; set; }
            public DateOnly дата_смены { get; set; }
            public TimeOnly начало_смены { get; set; }
            public TimeOnly конец_смены { get; set; }
            public string Duration { get; set; }
        }

        public ShiftsPage()
        {
            InitializeComponent();
            LoadShifts();
        }

        private void LoadShifts()
        {
            try
            {
                var shifts = db.Сменыs.ToList();
                var employees = db.Сотрудникs.ToDictionary(e => e.IdСотрудника, e => e);
                var positions = db.Должностиs.ToDictionary(p => p.IdДолжности, p => p.НаименованиеДолжности);

                var viewModels = shifts.Select(s => new ShiftViewModel
                {
                    id_смены = s.IdСмены,
                    EmployeeName = s.IdСотрудника.HasValue && employees.ContainsKey(s.IdСотрудника.Value) 
                        ? $"{employees[s.IdСотрудника.Value].Фамилия} {employees[s.IdСотрудника.Value].Имя}"
                        : "Не найден",
                    Position = s.IdСотрудника.HasValue && employees.ContainsKey(s.IdСотрудника.Value)
                        && employees[s.IdСотрудника.Value].IdДолжности.HasValue
                        ? (positions.ContainsKey(employees[s.IdСотрудника.Value].IdДолжности.Value)
                            ? positions[employees[s.IdСотрудника.Value].IdДолжности.Value]
                            : "Не указана")
                        : "Не указана",
                    дата_смены = s.ДатаСмены,
                    начало_смены = s.НачалоСмены,
                    конец_смены = s.КонецСмены,
                    Duration = CalculateDuration(s.НачалоСмены, s.КонецСмены)
                })
                .OrderByDescending(s => s.дата_смены)
                .ThenBy(s => s.начало_смены)
                .ToList();

                Shifts = new ObservableCollection<ShiftViewModel>(viewModels);
                ShiftsDataGrid.ItemsSource = Shifts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки смен: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string CalculateDuration(TimeOnly start, TimeOnly end)
        {
            var startTime = start.ToTimeSpan();
            var endTime = end.ToTimeSpan();

            if (endTime < startTime) // если смена через полночь
                endTime = endTime.Add(TimeSpan.FromHours(24));

            var duration = endTime - startTime;
            int hours = (int)duration.TotalHours;
            int minutes = duration.Minutes;

            return minutes > 0 ? $"{hours} ч {minutes} мин" : $"{hours} ч";
        }

        private void BtnAddShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Window
                {
                    Title = "Добавление смены",
                    Width = 450,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this),
                    Background = System.Windows.Media.Brushes.White
                };

                var stackPanel = new StackPanel { Margin = new Thickness(20) };

                // Выбор сотрудника
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Сотрудник:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 5)
                });

                var cmbEmployee = new ComboBox
                {
                    Margin = new Thickness(0, 0, 0, 15),
                    Height = 35,
                    DisplayMemberPath = "FullName"
                };

                var employees = db.Сотрудникs.ToList()
                    .Select(s => new
                    {
                        Id = s.IdСотрудника,
                        FullName = $"{s.Фамилия} {s.Имя} {s.Отчество}"
                    }).ToList();

                cmbEmployee.ItemsSource = employees;
                if (employees.Any()) cmbEmployee.SelectedIndex = 0;
                stackPanel.Children.Add(cmbEmployee);

                // Дата смены
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Дата смены:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 5)
                });

                var datePicker = new DatePicker
                {
                    Margin = new Thickness(0, 0, 0, 15),
                    Height = 35,
                    SelectedDate = DateTime.Today
                };
                stackPanel.Children.Add(datePicker);

                // Начало смены
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Начало смены (ЧЧ:ММ):",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 5)
                });

                var txtStartTime = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 15),
                    Height = 35,
                    Text = "09:00"
                };
                stackPanel.Children.Add(txtStartTime);

                // Конец смены
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Конец смены (ЧЧ:ММ):",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 5)
                });

                var txtEndTime = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 20),
                    Height = 35,
                    Text = "18:00"
                };
                stackPanel.Children.Add(txtEndTime);

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
                    try
                    {
                        var selectedEmployee = cmbEmployee.SelectedItem as dynamic;
                        if (selectedEmployee == null)
                        {
                            MessageBox.Show("Выберите сотрудника");
                            return;
                        }

                        if (!datePicker.SelectedDate.HasValue)
                        {
                            MessageBox.Show("Выберите дату");
                            return;
                        }

                        if (!TimeSpan.TryParse(txtStartTime.Text, out var startTime) ||
                            !TimeSpan.TryParse(txtEndTime.Text, out var endTime))
                        {
                            MessageBox.Show("Некорректный формат времени. Используйте ЧЧ:ММ");
                            return;
                        }

                        var newShift = new Смены
                        {
                            IdСотрудника = selectedEmployee.Id, // int? принимает int
                            ДатаСмены = DateOnly.FromDateTime(datePicker.SelectedDate.Value),
                            НачалоСмены = TimeOnly.FromTimeSpan(startTime),
                            КонецСмены = TimeOnly.FromTimeSpan(endTime)
                        };

                        db.Сменыs.Add(newShift);
                        db.SaveChanges();

                        dialog.Close();
                        LoadShifts();

                        MessageBox.Show("Смена добавлена", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                };

                btnCancel.Click += (s, args) => dialog.Close();

                var buttonStyle = new Style(typeof(Border));
                buttonStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(5)));
                btnSave.Resources.Add(typeof(Border), buttonStyle);
                btnCancel.Resources.Add(typeof(Border), buttonStyle);

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

        private void BtnEditShift_Click(object sender, RoutedEventArgs e)
        {
            var selectedShift = ShiftsDataGrid.SelectedItem as ShiftViewModel;

            if (selectedShift == null)
            {
                MessageBox.Show("Выберите смену для редактирования",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var shift = db.Сменыs.FirstOrDefault(s => s.IdСмены == selectedShift.id_смены);
                if (shift == null)
                {
                    MessageBox.Show("Смена не найдена в базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dialog = new Window
                {
                    Title = "Редактирование смены",
                    Width = 450,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this),
                    Background = System.Windows.Media.Brushes.White
                };

                var stackPanel = new StackPanel { Margin = new Thickness(20) };

                // Выбор сотрудника
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Сотрудник:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 5)
                });

                var cmbEmployee = new ComboBox
                {
                    Margin = new Thickness(0, 0, 0, 15),
                    Height = 35,
                    DisplayMemberPath = "FullName"
                };

                var employees = db.Сотрудникs.ToList()
                    .Select(s => new
                    {
                        Id = s.IdСотрудника,
                        FullName = $"{s.Фамилия} {s.Имя} {s.Отчество}"
                    }).ToList();

                cmbEmployee.ItemsSource = employees;
                cmbEmployee.SelectedItem = employees.FirstOrDefault(e => e.Id == shift.IdСотрудника);
                stackPanel.Children.Add(cmbEmployee);

                // Дата смены
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Дата смены:",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 5)
                });

                var datePicker = new DatePicker
                {
                    Margin = new Thickness(0, 0, 0, 15),
                    Height = 35,
                    SelectedDate = shift.ДатаСмены.ToDateTime(TimeOnly.MinValue)
                };
                stackPanel.Children.Add(datePicker);

                // Начало смены
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Начало смены (ЧЧ:ММ):",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 5)
                });

                var txtStartTime = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 15),
                    Height = 35,
                    Text = shift.НачалоСмены.ToString("HH:mm")
                };
                stackPanel.Children.Add(txtStartTime);

                // Конец смены
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Конец смены (ЧЧ:ММ):",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Orange,
                    Margin = new Thickness(0, 5, 0, 5)
                });

                var txtEndTime = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 20),
                    Height = 35,
                    Text = shift.КонецСмены.ToString("HH:mm")
                };
                stackPanel.Children.Add(txtEndTime);

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
                    try
                    {
                        var selectedEmployee = cmbEmployee.SelectedItem as dynamic;
                        if (selectedEmployee == null)
                        {
                            MessageBox.Show("Выберите сотрудника");
                            return;
                        }

                        if (!datePicker.SelectedDate.HasValue)
                        {
                            MessageBox.Show("Выберите дату");
                            return;
                        }

                        if (!TimeSpan.TryParse(txtStartTime.Text, out var startTime) ||
                            !TimeSpan.TryParse(txtEndTime.Text, out var endTime))
                        {
                            MessageBox.Show("Некорректный формат времени. Используйте ЧЧ:ММ");
                            return;
                        }

                        // Сохраняем изменения
                        shift.IdСотрудника = selectedEmployee.Id;
                        shift.ДатаСмены = DateOnly.FromDateTime(datePicker.SelectedDate.Value);
                        shift.НачалоСмены = TimeOnly.FromTimeSpan(startTime);
                        shift.КонецСмены = TimeOnly.FromTimeSpan(endTime);

                        db.SaveChanges();
                        dialog.Close();
                        LoadShifts();

                        MessageBox.Show("Смена обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
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

        private void BtnDeleteShift_Click(object sender, RoutedEventArgs e)
        {
            var selectedShift = ShiftsDataGrid.SelectedItem as ShiftViewModel;
            if (selectedShift == null)
            {
                MessageBox.Show("Выберите смену для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить смену №{selectedShift.id_смены}?\nСотрудник: {selectedShift.EmployeeName}\nДата: {selectedShift.дата_смены:dd.MM.yyyy}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var shiftToDelete = db.Сменыs.FirstOrDefault(s => s.IdСмены == selectedShift.id_смены); // ИСПРАВЛЕНО: id_смены
                if (shiftToDelete != null)
                {
                    db.Сменыs.Remove(shiftToDelete);
                    db.SaveChanges();
                    LoadShifts();
                    MessageBox.Show("Смена удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadShifts();
        }
    }
}