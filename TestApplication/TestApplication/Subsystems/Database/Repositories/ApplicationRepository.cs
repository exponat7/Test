using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApplication.Models.Request;
using TestApplication.Models.RequestJournal;

namespace TestApplication.Subsystems.Database.Repositories
{
    public class ApplicationRepository : IDisposable
    {
        private readonly ApplicationContext _context;
        /// <summary>
        /// Конструктор
        /// </summary>
        public ApplicationRepository()
        {
            _context = new ApplicationContext();
        }
        /// <summary>
        /// Освобождаем ресурсы
        /// </summary>
        public void Dispose() => _context.Dispose();
        
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        #region Requests

        public IQueryable<Request> Requests => _context.Requests;

        public void AddRequest(Request request)
        {
            _context.Requests.Add(request);
            _context.SaveChanges();
        }

        #endregion

        #region RequestsJournals

        public IQueryable<RequestsJournalRecord> JournalRecords => _context.JournalRecords;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns>Request Id</returns>
        public void AddJournalRecord(RequestsJournalRecord record)
        {
            _context.Requests.Attach(record.Request);
            _context.JournalRecords.Add(record);
            _context.SaveChanges();
        }

        #endregion

    }
}
