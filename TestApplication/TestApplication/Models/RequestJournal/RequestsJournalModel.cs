using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplication.Models.RequestJournal
{
    public enum RequestState
    {
        //получен, но еще не обработан
        [Display(Name = "Новый")]
        Registered,
        //В работе у оператора
        [Display(Name = "В работе")]
        Assigned,
        //Успешно завершен
        [Display(Name = "Обработан")]
        Finalized,
        //Прерван клиентом
        [Display(Name = "Прерван")]
        Aborted,
        //В процессе обработки произошла ошибка
        [Display(Name = "Ошибка")]
        Error
    }

    /// <summary>
    /// Модель базы данных журнала звонков
    /// </summary>
    public class RequestsJournalRecord
    {
        private int _state;

        public int Id { get; set; }

        //public string UserId { get; set; }
        [Required]
        public DateTime StateChanged { get; set; }

        [Required]
        public Request.Request Request { get; set; }

        [Required]
        public int CurrentState
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
