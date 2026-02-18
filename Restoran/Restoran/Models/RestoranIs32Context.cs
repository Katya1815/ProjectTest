using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RestoranApi.Models;

public partial class RestoranIs32Context : DbContext
{
    public RestoranIs32Context()
    {
    }

    public RestoranIs32Context(DbContextOptions<RestoranIs32Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Блюдо> Блюдоs { get; set; }

    public virtual DbSet<Должности> Должностиs { get; set; }

    public virtual DbSet<Заказ> Заказs { get; set; }

    public virtual DbSet<Клиент> Клиентs { get; set; }

    public virtual DbSet<Меню> Менюs { get; set; }

    public virtual DbSet<Роли> Ролиs { get; set; }

    public virtual DbSet<Смены> Сменыs { get; set; }

    public virtual DbSet<Сотрудник> Сотрудникs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=RestoranIS32;Username=postgres;Password=1111");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Блюдо>(entity =>
        {
            entity.HasKey(e => e.IdБлюда).HasName("Блюдо_pkey");

            entity.ToTable("Блюдо");

            entity.Property(e => e.IdБлюда).HasColumnName("id_блюда");
            entity.Property(e => e.КатегорияБлюда)
                .HasMaxLength(100)
                .HasColumnName("категория_блюда");
            entity.Property(e => e.Наименование)
                .HasMaxLength(200)
                .HasColumnName("наименование");
            entity.Property(e => e.ОписаниеБлюда).HasColumnName("описание_блюда");
            entity.Property(e => e.Цена)
                .HasPrecision(10, 2)
                .HasColumnName("цена");
        });

        modelBuilder.Entity<Должности>(entity =>
        {
            entity.HasKey(e => e.IdДолжности).HasName("Должности_pkey");

            entity.ToTable("Должности");

            entity.Property(e => e.IdДолжности).HasColumnName("id_должности");
            entity.Property(e => e.НаименованиеДолжности)
                .HasMaxLength(100)
                .HasColumnName("наименование_должности");
            entity.Property(e => e.Описание).HasColumnName("описание");
            entity.Property(e => e.УровеньДоступа).HasColumnName("уровень_доступа");
        });

        modelBuilder.Entity<Заказ>(entity =>
        {
            entity.HasKey(e => e.IdЗаказа).HasName("Заказ_pkey");

            entity.ToTable("Заказ");

            entity.Property(e => e.IdЗаказа).HasColumnName("id_заказа");
            entity.Property(e => e.ВремяПринятияЗаказа).HasColumnName("время_принятия_заказа");
            entity.Property(e => e.ДатаПринятияЗаказа).HasColumnName("дата_принятия_заказа");
            entity.Property(e => e.Заказчик).HasColumnName("заказчик");
            entity.Property(e => e.НомерСтолика).HasColumnName("номер_столика");
            entity.Property(e => e.ОписаниеЗаказа).HasColumnName("описание_заказа");
            entity.Property(e => e.Оплачено)
                .HasDefaultValue(false)
                .HasColumnName("оплачено");
            entity.Property(e => e.Официант).HasColumnName("официант");
            entity.Property(e => e.Статус)
                .HasMaxLength(50)
                .HasColumnName("статус");
            entity.Property(e => e.Сумма)
                .HasPrecision(10, 2)
                .HasColumnName("сумма");

            entity.HasOne(d => d.ЗаказчикNavigation).WithMany(p => p.Заказs)
                .HasForeignKey(d => d.Заказчик)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_заказ_клиент");

            entity.HasOne(d => d.ОписаниеЗаказаNavigation).WithMany(p => p.Заказs)
                .HasForeignKey(d => d.ОписаниеЗаказа)
                .HasConstraintName("fk_заказ_меню");

            entity.HasOne(d => d.ОфициантNavigation).WithMany(p => p.Заказs)
                .HasForeignKey(d => d.Официант)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_заказ_официант");
        });

        modelBuilder.Entity<Клиент>(entity =>
        {
            entity.HasKey(e => e.IdКлиента).HasName("Клиент_pkey");

            entity.ToTable("Клиент");

            entity.Property(e => e.IdКлиента).HasColumnName("id_клиента");
            entity.Property(e => e.IdРоли).HasColumnName("id_роли");
            entity.Property(e => e.ДатаРегистрацииВСистеме).HasColumnName("дата_регистрации_в_системе");
            entity.Property(e => e.Имя)
                .HasMaxLength(100)
                .HasColumnName("имя");
            entity.Property(e => e.Отчество)
                .HasMaxLength(100)
                .HasColumnName("отчество");
            entity.Property(e => e.Примечания)
                .HasMaxLength(300)
                .HasColumnName("примечания");
            entity.Property(e => e.Фамилия)
                .HasMaxLength(100)
                .HasColumnName("фамилия");

            entity.HasOne(d => d.IdРолиNavigation).WithMany(p => p.Клиентs)
                .HasForeignKey(d => d.IdРоли)
                .HasConstraintName("fk_клиент_роли");
        });

        modelBuilder.Entity<Меню>(entity =>
        {
            entity.HasKey(e => e.IdМеню).HasName("Меню_pkey");

            entity.ToTable("Меню");

            entity.Property(e => e.IdМеню).HasColumnName("id_меню");
            entity.Property(e => e.IdБлюда).HasColumnName("id_блюда");
            entity.Property(e => e.ДатаДобавления).HasColumnName("дата_добавления");
            entity.Property(e => e.ДоступноДляЗаказа)
                .HasDefaultValue(true)
                .HasColumnName("доступно_для_заказа");

            entity.HasOne(d => d.IdБлюдаNavigation).WithMany(p => p.Менюs)
                .HasForeignKey(d => d.IdБлюда)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_меню_блюдо");
        });

        modelBuilder.Entity<Роли>(entity =>
        {
            entity.HasKey(e => e.IdРоли).HasName("Роли_pkey");

            entity.ToTable("Роли");

            entity.Property(e => e.IdРоли).HasColumnName("id_роли");
            entity.Property(e => e.НаименованиеРоли)
                .HasMaxLength(100)
                .HasColumnName("наименование_роли");
        });

        modelBuilder.Entity<Смены>(entity =>
        {
            entity.HasKey(e => e.IdСмены).HasName("Смены_pkey");

            entity.ToTable("Смены");

            entity.Property(e => e.IdСмены).HasColumnName("id_смены");
            entity.Property(e => e.IdСотрудника).HasColumnName("id_сотрудника");
            entity.Property(e => e.ДатаСмены).HasColumnName("дата_смены");
            entity.Property(e => e.КонецСмены).HasColumnName("конец_смены");
            entity.Property(e => e.НачалоСмены).HasColumnName("начало_смены");

            entity.HasOne(d => d.IdСотрудникаNavigation).WithMany(p => p.Сменыs)
                .HasForeignKey(d => d.IdСотрудника)
                .HasConstraintName("fk_смены_сотрудник");
        });

        modelBuilder.Entity<Сотрудник>(entity =>
        {
            entity.HasKey(e => e.IdСотрудника).HasName("Сотрудник_pkey");

            entity.ToTable("Сотрудник");

            entity.Property(e => e.IdСотрудника).HasColumnName("id_сотрудника");
            entity.Property(e => e.IdДолжности).HasColumnName("id_должности");
            entity.Property(e => e.IdРоли).HasColumnName("id_роли");
            entity.Property(e => e.ДатаРождения).HasColumnName("дата_рождения");
            entity.Property(e => e.Имя)
                .HasMaxLength(100)
                .HasColumnName("имя");
            entity.Property(e => e.Логин)
                .HasMaxLength(255)
                .HasColumnName("логин");
            entity.Property(e => e.НомерТелефона)
                .HasMaxLength(20)
                .HasColumnName("номер_телефона");
            entity.Property(e => e.Отчество)
                .HasMaxLength(100)
                .HasColumnName("отчество");
            entity.Property(e => e.Пароль)
                .HasMaxLength(255)
                .HasColumnName("пароль");
            entity.Property(e => e.Фамилия)
                .HasMaxLength(100)
                .HasColumnName("фамилия");

            entity.HasOne(d => d.IdДолжностиNavigation).WithMany(p => p.Сотрудникs)
                .HasForeignKey(d => d.IdДолжности)
                .HasConstraintName("fk_сотрудник_должности");

            entity.HasOne(d => d.IdРолиNavigation).WithMany(p => p.Сотрудникs)
                .HasForeignKey(d => d.IdРоли)
                .HasConstraintName("fk_сотрудник_роли");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
