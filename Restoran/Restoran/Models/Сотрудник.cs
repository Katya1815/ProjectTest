using System;
using System.Collections.Generic;

namespace RestoranApi.Models;

public partial class Сотрудник
{
    public int IdСотрудника { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчество { get; set; }

    public string Пароль { get; set; } = null!;

    public string Логин { get; set; } = null!;

    public DateOnly? ДатаРождения { get; set; }

    public int? IdДолжности { get; set; }

    public string? НомерТелефона { get; set; }

    public int? IdРоли { get; set; }

    public virtual Должности? IdДолжностиNavigation { get; set; }

    public virtual Роли? IdРолиNavigation { get; set; }

    public virtual ICollection<Заказ> Заказs { get; set; } = new List<Заказ>();

    public virtual ICollection<Смены> Сменыs { get; set; } = new List<Смены>();
}
