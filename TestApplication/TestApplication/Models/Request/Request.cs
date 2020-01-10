using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TestApplication.Models.RequestJournal;

namespace TestApplication.Models.Request
{
    public class Request
    {
        private int _state;

        public int Id { get; set; }

        public string IncomingIdentifier { get; set; }//Phone Number как пример

        public string UserId { get; set; }//Так как обрабатывается запрос однозначно одним пользователем, либо никем, то в журнал нет смысла писать

        //Навигационное свойство
        public ICollection<RequestsJournalRecord> JournalRecords { get; set; }

        public int LastState
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        [NotMapped]
        public RequestState State
        {
            get
            {
                return (RequestState)_state;
            }
            set
            {
                _state = (int)value;
            }
        }
    }
}
