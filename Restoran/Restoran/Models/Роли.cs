using System;
using System.Collections.Generic;

namespace RestoranApi.Models;

public partial class Роли
{
    public int IdРоли { get; set; }

    public string НаименованиеРоли { get; set; } = null!;

    public virtual ICollection<Клиент> Клиентs { get; set; } = new List<Клиент>();

    public virtual ICollection<Сотрудник> Сотрудникs { get; set; } = new List<Сотрудник>();
}
