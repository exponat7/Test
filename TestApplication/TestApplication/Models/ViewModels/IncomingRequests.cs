using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApplication.Models.RequestJournal;

namespace TestApplication.Models.ViewModels
{
    public class IncomingRequests
    {
        public int IncomingCount { get; set; }

        public List<JournalRecord> Records { get; set; }

        public IncomingRequests()
        {
            Records = new List<JournalRecord>();
        }
    }

    public class JournalRecord
    {

        public int Id { get; set; }
        public string UserName { get; set; }

        public string IncomingIdentifier { get; set; }

        public RequestState LastState { get; set; }

        public DateTime RegisterTime { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
