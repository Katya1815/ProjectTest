using System;
using System.Collections.Generic;

namespace RestoranApi.Models;

public partial class Блюдо
{
    public int IdБлюда { get; set; }

    public string Наименование { get; set; } = null!;

    public string? ОписаниеБлюда { get; set; }

    public decimal Цена { get; set; }

    public string? КатегорияБлюда { get; set; }

    public virtual ICollection<Меню> Менюs { get; set; } = new List<Меню>();
}
