using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using TestApplication.Areas.Identity.Models;

namespace TestApplication.Areas.Identity.Subsystems.Database
{
    /// <summary>
    /// Контекст для работы со встроенной системой Identity
    /// </summary>
    public class IdentityContext : IdentityDbContext<CustomIdentityUser>
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public IdentityContext()
        {
        }

        /// <summary>
        /// Конструктор принимающий опции контекста
        /// </summary>
        /// <param name="options">Опции контекста</param>
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Метод настройки подсиситемы Identity
        /// </summary>
        /// <param name="optionsBuilder">Объект настроек</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(Startup.Configuration.GetConnectionString("IdentityConnection"));
            }
        }

        /// <summary>
        /// Метод сопоставления модели с таблицей в базе данных
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
