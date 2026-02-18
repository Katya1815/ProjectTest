using System;
using System.Collections.Generic;

namespace RestoranApi.Models;

public partial class Смены
{
    public int IdСмены { get; set; }

    public int? IdСотрудника { get; set; }

    public DateOnly ДатаСмены { get; set; }

    public TimeOnly НачалоСмены { get; set; }

    public TimeOnly КонецСмены { get; set; }

    public virtual Сотрудник? IdСотрудникаNavigation { get; set; }
}
