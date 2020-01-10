using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestApplication.Models.Request;
using TestApplication.Models.RequestJournal;

namespace TestApplication.Subsystems.Database
{
    /// <summary>
    /// Системный контекст предназначен для хранения данных приложения
    /// </summary>
    public class ApplicationContext : DbContext
    {
        /// <summary>
        ///Конструктор
        /// </summary>
        public ApplicationContext()
        {

        }

        /// <summary>
        /// Конструктор с опциями
        /// </summary>
        /// <param name="options">Настройки</param>
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {

        }

        /// <summary>
        /// Конфигурация системного контекста работы с БД
        /// </summary>
        /// <param name="optionsBuilder">Настройки</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(Startup.Configuration.GetConnectionString("ApplicationConnection"));
            }
        }

        public virtual DbSet<Request> Requests { get; set; }

        public virtual DbSet<RequestsJournalRecord> JournalRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>()
                .HasMany(r => r.JournalRecords)
                .WithOne(jr => jr.Request);
        }
    }
}