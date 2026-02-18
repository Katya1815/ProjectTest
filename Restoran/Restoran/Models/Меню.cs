using System;
using System.Collections.Generic;

namespace RestoranApi.Models;

public partial class Меню
{
    public int IdМеню { get; set; }

    public int IdБлюда { get; set; }

    public DateOnly ДатаДобавления { get; set; }

    public bool? ДоступноДляЗаказа { get; set; }

    public virtual Блюдо IdБлюдаNavigation { get; set; } = null!;

    public virtual ICollection<Заказ> Заказs { get; set; } = new List<Заказ>();
}
