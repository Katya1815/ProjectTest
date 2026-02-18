using System;
using System.Collections.Generic;

namespace RestoranApi.Models;

public partial class Должности
{
    public int IdДолжности { get; set; }

    public string НаименованиеДолжности { get; set; } = null!;

    public string? Описание { get; set; }

    public int УровеньДоступа { get; set; }

    public virtual ICollection<Сотрудник> Сотрудникs { get; set; } = new List<Сотрудник>();
}
