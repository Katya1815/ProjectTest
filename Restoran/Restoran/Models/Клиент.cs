using System;
using System.Collections.Generic;

namespace RestoranApi.Models;

public partial class Клиент
{
    public int IdКлиента { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчество { get; set; }

    public DateOnly ДатаРегистрацииВСистеме { get; set; }

    public string? Примечания { get; set; }

    public int? IdРоли { get; set; }

    public virtual Роли? IdРолиNavigation { get; set; }

    public virtual ICollection<Заказ> Заказs { get; set; } = new List<Заказ>();
}
