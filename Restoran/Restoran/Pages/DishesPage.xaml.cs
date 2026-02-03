using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для DishesPage.xaml
    /// </summary>
    public partial class DishesPage : Page
    {
        private List<Блюдо> _allDishes;

        public DishesPage()
        {
            InitializeComponent();
            LoadDishes();
        }

        private void LoadDishes()
        {
            try
            {
                using (var context = App.Context)
                {
                    _allDishes = context.Блюда
                        .OrderBy(d => d.Категория_блюда)
                        .ThenBy(d => d.Наименование)
                        .ToList();

                    DishesGrid.ItemsSource = _allDishes;

                    // Заполняем фильтр категорий
                    var categories = _allDishes
                        .Select(d => d.Категория_блюда)
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList();

                    CategoryFilter.Items.Clear();
                    CategoryFilter.Items.Add("Все категории");
                    foreach (var category in categories)
                    {
                        CategoryFilter.Items.Add(category);
                    }
                    CategoryFilter.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки блюд: {ex.Message}");
            }
        }

        private void AddDish_Click(object sender, RoutedEventArgs e)
        {
            var newDish = new Блюдо
            {
                Наименование = "Новое блюдо",
                Категория_блюда = "Основные блюда",
                Цена = 0,
                Описание_блюда = ""
            };

            _allDishes.Add(newDish);
            DishesGrid.ItemsSource = null;
            DishesGrid.ItemsSource = _allDishes;
            DishesGrid.SelectedItem = newDish;
            DishesGrid.ScrollIntoView(newDish);

            // Переходим в режим редактирования
            var cell = DataGridHelper.GetCell(DishesGrid, _allDishes.Count - 1, 1);
            if (cell != null) cell.Focus();
        }

        private void EditDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int dishId)
            {
                var dish = _allDishes.FirstOrDefault(d => d.Id_блюда == dishId);
                if (dish != null)
                {
                    DishesGrid.SelectedItem = dish;
                    DishesGrid.ScrollIntoView(dish);
                    DishesGrid.BeginEdit();
                }
            }
        }

        private void DeleteDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int dishId)
            {
                var dish = _allDishes.FirstOrDefault(d => d.Id_блюда == dishId);
                if (dish != null)
                {
                    if (MessageBox.Show($"Удалить блюдо \"{dish.Наименование}\"?",
                        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            using (var context = App.Context)
                            {
                                var dishToDelete = context.Блюда.Find(dishId);
                                if (dishToDelete != null)
                                {
                                    context.Блюда.Remove(dishToDelete);
                                    context.SaveChanges();

                                    _allDishes.Remove(dish);
                                    DishesGrid.ItemsSource = null;
                                    DishesGrid.ItemsSource = _allDishes;

                                    MessageBox.Show("Блюдо успешно удалено", "Успех");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка");
                        }
                    }
                }
            }
        }

        private void SearchDishBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterDishes();
        }

        private void FilterDishes(object sender = null, SelectionChangedEventArgs e = null)
        {
            if (_allDishes == null) return;

            var filtered = _allDishes.AsQueryable();

            // Поиск по названию
            if (!string.IsNullOrWhiteSpace(SearchDishBox.Text))
            {
                string searchText = SearchDishBox.Text.ToLower();
                filtered = filtered.Where(d => d.Наименование.ToLower().Contains(searchText) ||
                                               d.Описание_блюда.ToLower().Contains(searchText));
            }

            // Фильтр по категории
            if (CategoryFilter.SelectedItem != null && CategoryFilter.SelectedItem.ToString() != "Все категории")
            {
                string selectedCategory = CategoryFilter.SelectedItem.ToString();
                filtered = filtered.Where(d => d.Категория_блюда == selectedCategory);
            }

            DishesGrid.ItemsSource = filtered.ToList();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DishesGrid.CommitEdit(); // Сохраняем изменения в DataGrid

                using (var context = App.Context)
                {
                    // Сохраняем изменения для каждого блюда
                    foreach (var dish in _allDishes)
                    {
                        if (dish.Id_блюда == 0) // Новое блюдо
                        {
                            context.Блюда.Add(dish);
                        }
                        else // Существующее блюдо
                        {
                            context.Entry(dish).State = EntityState.Modified;
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Изменения успешно сохранены!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new WaiterMenuPage());
        }
    }

    // Вспомогательный класс для работы с DataGrid
    public static class DataGridHelper
    {
        public static DataGridCell GetCell(DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(row, dataGrid.Columns[columnIndex]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        private static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}