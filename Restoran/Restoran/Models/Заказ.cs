using System;
using System.Collections.Generic;

namespace RestoranApi.Models;

public partial class Заказ
{
    public int IdЗаказа { get; set; }

    public int НомерСтолика { get; set; }

    public int Заказчик { get; set; }

    public int Официант { get; set; }

    public DateOnly ДатаПринятияЗаказа { get; set; }

    public TimeOnly ВремяПринятияЗаказа { get; set; }

    public int? ОписаниеЗаказа { get; set; }

    public string Статус { get; set; } = null!;

    public decimal Сумма { get; set; }

    public bool? Оплачено { get; set; }

    public virtual Клиент ЗаказчикNavigation { get; set; } = null!;

    public virtual Меню? ОписаниеЗаказаNavigation { get; set; }

    public virtual Сотрудник ОфициантNavigation { get; set; } = null!;
}
