using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplication.Areas.Identity.Models
{
    /// <summary>
    /// Переопределенный класс для добавления прикладных свойств в IdentityUser
    /// </summary>
    public class CustomIdentityUser : IdentityUser
    {
        /// <summary>
        /// Состояние учетной записи пользователя (подключен/отключен)
        /// </summary>
        public bool IsActive { get; set; }

        public bool IsFree { get; set; }
       
        public bool IsLoggedIn { get; set; }

        public DateTime? LastCallStart { get; set; }
    }
}
