using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplication.Models.ViewModels
{
    public class SettingsModel
    {
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения")]
        [Display(Name = "Период перехода запроса на менеджера(сек):")]
        public int? Tm { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения")]
        [Display(Name = "Период перехода запроса на директора(сек):")]
        public int? Td { get; set; }//Не должен быть меньше Tm

        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения")]
        [Display(Name = "Минимальный период возможного завершения обработки:")]
        public int? From { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения")]
        [Display(Name = "Максимальный период возможного завершения обработки:")]
        public int? To { get; set; }//Не меньше From
    }
}
