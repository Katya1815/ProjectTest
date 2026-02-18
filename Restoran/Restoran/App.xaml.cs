using System.Windows;
using RestoranApi.Models;

namespace Restoran
{
    public partial class App : Application
    {
        public static RestoranIs32Context Context { get; set; } = new RestoranIs32Context();
        public static Сотрудник? CurrentUser { get; set; } = null;
        public static Клиент? CurrentClient { get; set; } = null;
    }
}