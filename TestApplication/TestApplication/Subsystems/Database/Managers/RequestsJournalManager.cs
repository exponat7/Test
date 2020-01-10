using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApplication.Models.Request;
using TestApplication.Models.RequestJournal;
using TestApplication.Subsystems.Database.Repositories;

namespace TestApplication.Subsystems.Database.Managers
{
    public class RequestsJournalManager : BaseManager
    {
        //public List<RequestsJournalRecord> GetOrderedJournal() => _repo.JournalRecords.Include(jr=>jr.Request).OrderByDescending(r => r.Id).ToList();

        //public List<RequestsJournalRecord> GetOrderedJournalForUser(string userId) => _repo.JournalRecords.Include(jr => jr.Request).Where(jr=>jr.Request.UserId==userId).OrderByDescending(r => r.Id).ToList();

        public void AddRecord(RequestsJournalRecord record) => _repo.AddJournalRecord(record);

        public List<RequestsJournalRecord> GetJournalRecords => _repo.JournalRecords.ToList();

        //public RequestsJournalRecord FindLastRecordForRequest(Request request) => _repo.JournalRecords.Where(rj => rj.Request.Id == request.Id).OrderBy(rj => rj.CurrentState).Last();

        public DateTime GetRegisterTimeForRequest(Request request) => _repo.JournalRecords.First(rj => rj.Request.Id == request.Id && rj.CurrentState == (int)RequestState.Registered).StateChanged;

        public List<RequestsJournalRecord> GetJournalRecordsForUser(string userId) => _repo.JournalRecords.Where(jr=>jr.Request.UserId==userId).ToList();
    }
}
