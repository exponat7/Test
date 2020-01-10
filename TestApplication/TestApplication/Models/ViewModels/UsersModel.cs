using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplication.Models.ViewModels
{
    public enum UserState
    {
        [Display(Name = "Не в системе")]
        NotLoggedIn,
        [Display(Name = "Свободен")]
        Free,
        [Display(Name = "Занят")]
        Busy,

    }

    public class UsersModel
    {
        public List<SingleUser> Users { get; set; }

        public UsersModel()
        {
            Users = new List<SingleUser>();
        }
    }

    public class SingleUser
    {
        public string Id { get; set; }
        [Display(Name ="Пользователь")]
        public string UserName { get; set; }
        [Display(Name = "Текущий статус")]
        public UserState CurrentState { get; set; }
    }
}
