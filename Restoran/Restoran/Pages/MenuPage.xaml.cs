using RestoranApi.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restoran.Pages
{
    public partial class MenuPage : Page
    {
        private RestoranIs32Context db = new RestoranIs32Context();
        private ObservableCollection<Блюдо> AllDishes { get; set; }
        private ObservableCollection<Блюдо> FilteredDishes { get; set; }

        public MenuPage()
        {
            InitializeComponent();
            LoadMenu();
        }

        private void LoadMenu()
        {
            try
            {
                var dishes = db.Блюдоs.ToList();
                AllDishes = new ObservableCollection<Блюдо>(dishes);
                FilteredDishes = new ObservableCollection<Блюдо>(dishes);
                MenuItemsControl.ItemsSource = FilteredDishes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки меню: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterByCategory(string category)
        {
            try
            {
                if (category == "Все")
                {
                    FilteredDishes = new ObservableCollection<Блюдо>(AllDishes);
                }
                else
                {
                    var filtered = AllDishes.Where(d => d.КатегорияБлюда == category).ToList();
                    FilteredDishes = new ObservableCollection<Блюдо>(filtered);
                }

                MenuItemsControl.ItemsSource = FilteredDishes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAllCategories_Click(object sender, RoutedEventArgs e) => FilterByCategory("Все");
        private void BtnPizzaCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Пицца");
        private void BtnPastaCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Паста");
        private void BtnSaladsCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Салаты");
        private void BtnSoupsCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Супы");
        private void BtnRollsCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Роллы");
        private void BtnBurgersCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Бургеры");
        private void BtnSidesCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Гарниры");
        private void BtnDessertsCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Десерты");
        private void BtnDrinksCategory_Click(object sender, RoutedEventArgs e) => FilterByCategory("Напитки");
    }
}