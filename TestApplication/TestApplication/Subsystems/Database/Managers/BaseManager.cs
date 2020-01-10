using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApplication.Subsystems.Database.Repositories;

namespace TestApplication.Subsystems.Database.Managers
{
    public class BaseManager : IDisposable
    {
        protected readonly ApplicationRepository _repo;
        /// <summary>
        /// Конструктор
        /// </summary>
        public BaseManager()
        {
            _repo = new ApplicationRepository();
        }
        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose() => _repo.Dispose();
    }
}
