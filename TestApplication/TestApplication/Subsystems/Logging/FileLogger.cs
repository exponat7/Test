using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplication.Subsystems.Logging
{
    /// <summary>
    /// Подсистема логирования, хранение данных в файлах
    /// </summary>
    public class FileLogger : ILogger
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        private string filePath;
        /// <summary>
        /// Полный путь, включая имя файла
        /// </summary>
        private string fullPath;
        /// <summary>
        /// Локер для подсистемы логирования
        /// </summary>
        private object _lock = new object();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="name">Имя файла</param>
        public FileLogger(string path, string name)
        {
            filePath = path;
            fullPath = Path.Combine(path, $"{name}.log");
        }
        /// <summary>
        /// Реализация интерфейса ILogger
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        /// <summary>
        /// Реализация интерфейса ILogger
        /// </summary>
        /// <param name="logLevel">Уровень логирования</param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        /// <summary>
        /// Реализация интерфейса ILogger
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel">Уровень логирования</param>
        /// <param name="eventId">Идентификатор события</param>
        /// <param name="state">Состояние</param>
        /// <param name="exception">Исключение, ошибка</param>
        /// <param name="formatter">Функция форматирвоания текста</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    //Создаем директорию логирования(нужно при каждом обращении, так как удалить папку могут в любое время)
                    Directory.CreateDirectory(filePath);
                    //
                    File.AppendAllText(fullPath, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}:{Environment.NewLine}{formatter(state, exception)}{Environment.NewLine}");
                }
            }
        }
    }

    /// <summary>
    /// Провайдер файлового логирования
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        private string path;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_path">Путь к файлу логирования</param>
        public FileLoggerProvider(string _path)
        {
            path = _path;
        }

        /// <summary>
        /// Создание логера
        /// </summary>
        /// <param name="categoryName">Категория уровня логирования</param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path, categoryName);
        }
        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Расширение для добавления стандартизированных функций инициализации файлового логирования
    /// </summary>
    public static class FileLoggerExtensions
    {
        /// <summary>
        /// Функция инициализации фабрики логирования
        /// </summary>
        /// <param name="factory">Фабрика логирования</param>
        /// <param name="filePath">Путь к файлу логирования</param>
        /// <returns></returns>
        public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath)
        {
            factory.AddProvider(new FileLoggerProvider(filePath));
            return factory;
        }
    }
}