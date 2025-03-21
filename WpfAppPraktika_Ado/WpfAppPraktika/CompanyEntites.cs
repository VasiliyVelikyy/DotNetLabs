﻿using System;
using System.Data.Entity;
using System.Linq;

namespace WpfAppPraktika
{
    using System.Data.Entity;
    using WpfAppPraktika.Model;

    public class CompanyEntities : DbContext
    {
        // Контекст настроен для использования строки подключения "CompanyEntities" из файла конфигурации
        // приложения (App.config или Web.config). По умолчанию эта  строка подключения указывает на базу данных
        // "WpfApplDemo2018.CompanyEntities" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "CompanyEntities" 
        // в файле конфигурации приложения.
        public CompanyEntities()
        : base("name=CompanyEntities")
        {
        }


        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель.Дополнительные сведения
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390107.
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
    }
    //public class MyEntity
    //{
    // public int Id { get; set; }
    // public string Name { get; set; }
    //}
}