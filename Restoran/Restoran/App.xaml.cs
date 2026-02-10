using System.Configuration;
using System.Data;
using System.Windows;

namespace Restoran
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ReastoranContext Context { get; } = new ReastoranContext();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Проверяем подключение к БД
            try
            {
                Context.Database.CanConnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД: {ex.Message}");
            }
        }
    }

}
