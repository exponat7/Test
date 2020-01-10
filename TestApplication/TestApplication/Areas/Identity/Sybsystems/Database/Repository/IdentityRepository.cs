using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApplication.Areas.Identity.Models;

namespace TestApplication.Areas.Identity.Subsystems.Database.Repository
{
    /// <summary>
    /// Класс реализующий репозиторий подсистемы Identity, предоставляет набор методов для работы с данными
    /// </summary>
    public class IdentityRepository
    {
        /// <summary>
        /// Контекст подсистемы Identity
        /// </summary>
        private readonly IdentityContext _context;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public IdentityRepository()
        {
            _context = new IdentityContext();
        }

        /// <summary>
        /// Метод получения списока всех пользователей
        /// </summary>
        /// <returns></returns>
        public List<CustomIdentityUser> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        /// <summary>
        /// Метод получения учетной записи пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя в БД</param>
        /// <returns></returns>
        public CustomIdentityUser GetUserById(string id)
        {
            return _context.Users.FirstOrDefault(u=>u.Id.Equals(id));
        }

        /// <summary>
        /// Метод изменения данных в учетной записи пользователя
        /// </summary>
        /// <param name="user">Модель учетной записи пользователя</param>
        public void UpdateUser(CustomIdentityUser user)
        {
            _context.Users.Attach(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Изменение состояния учетной записи
        /// </summary>
        /// <param name="user">Модель учетной записи пользователя</param>
        /// <param name="Status">Значение состояния</param>
        public void UpdateIsActiveStatus(CustomIdentityUser user, bool Status)
        {
            _context.Users.Attach(user);
            user.IsActive = Status;
            _context.SaveChanges();
        }

        /// <summary>
        /// Изменение состояния учетной записи
        /// </summary>
        /// <param name="user">Модель учетной записи пользователя</param>
        /// <param name="Status">Значение состояния</param>
        public void UpdateIsFreeStatus(CustomIdentityUser user, bool Status)
        {
            _context.Users.Attach(user);
            user.IsFree = Status;
            _context.SaveChanges();
        }

        /// <summary>
        /// Изменение состояния учетной записи
        /// </summary>
        /// <param name="user">Модель учетной записи пользователя</param>
        /// <param name="Status">Значение состояния</param>
        public void UpdateIsLoggedInStatus(CustomIdentityUser user, bool Status)
        {
            _context.Users.Attach(user);
            user.IsLoggedIn = Status;
            _context.SaveChanges();
        }

        /// <summary>
        /// Метод добавление роли в список существующих ролей приложения
        /// </summary>
        /// <param name="Name">Название роли</param>
        public void AddRole(string Name)
        {
            IdentityRole role = new IdentityRole
            {
                Name = Name,
                NormalizedName = Name.ToUpper()
            };
            _context.Roles.Add(role);
            _context.SaveChanges();
        }

        /// <summary>
        /// Метод проверки существования роли с указанным названием роли
        /// </summary>
        /// <param name="Name">Навание роли</param>
        /// <returns></returns>
        public bool RoleExist(string Name)
        {
            return _context.Roles.Any(m => m.Name == Name);
        }

        /// <summary>
        /// Метод для получения контекста роли по указанному идентификатору
        /// </summary>
        /// <param name="Id">Идентификатор в БД</param>
        /// <returns></returns>
        public IdentityRole GetRoleById(string Id)
        {
            return _context.Roles.SingleOrDefault(r => r.Id.Equals(Id));
        }

        /// <summary>
        /// Метод удаления роли
        /// </summary>
        /// <param name="Role">Контект роли</param>
        public void DeleteRole(IdentityRole Role)
        {
            _context.Roles.Remove(Role);
            _context.SaveChanges();
        }



        /// <summary>
        /// Метод получения списка ролей из учетной записи пользователя
        /// </summary>
        /// <param name="UserId">Идентификатор учетной записи пользователя в БД</param>
        /// <returns></returns>
        public List<IdentityRole> GetAllUserRoles(string UserId)
        {
            List<IdentityRole> retv = new List<IdentityRole>();
            var roles = _context.UserRoles.Where(u => u.UserId == UserId);
            foreach(var role in roles)
            {
                retv.Add(_context.Roles.FirstOrDefault(r => r.Id.Equals(role.RoleId)));
            }
            return retv;
        }

        /// <summary>
        /// Метод проверки наличия учетной записи пользователя с указанной ролью
        /// </summary>
        /// <param name="UserId">Идентифкатор учетной записи пользователя в БД</param>
        /// <param name="RoleId">Идентифкатор роли в БД</param>
        /// <returns></returns>
        public bool IsUserInRole(string UserId, string RoleId)
        {
            return _context.UserRoles.Any(r=>r.UserId==UserId && r.RoleId==RoleId);
        }

        public CustomIdentityUser GetFirstFreeUser(DateTime CallStart, int Tm, int Td)
        {
            CustomIdentityUser retv = null;
            var Elapsed = (DateTime.Now - CallStart).TotalMilliseconds;
            if (Elapsed >= Td)
            {
                //Любая роль получает звонок
                retv = _context.Users.FirstOrDefault(u => u.IsFree && u.IsLoggedIn);
            }
            else if (Elapsed >= Tm)
            {
                var opId = _context.Roles.First(r => r.Name == "operator").Id;
                var manId = _context.Roles.First(r => r.Name == "manager").Id;
                //Оператор либо менеджер
                var usrs = _context.UserRoles.Where(r => r.RoleId == opId || r.RoleId == manId).ToList();
                if (usrs != null && usrs.Count > 0)
                {
                    retv = _context.Users.FirstOrDefault(u => u.IsFree && u.IsLoggedIn && usrs.Any(ur => ur.UserId == u.Id));
                }
            }
            else
            {
                //Только оператор
                var opId = _context.Roles.First(r => r.Name == "operator").Id;
                //
                var usrs = _context.UserRoles.Where(r => r.RoleId == opId).ToList();
                if (usrs != null && usrs.Count > 0)
                {
                    retv = _context.Users.FirstOrDefault(u => u.IsFree && u.IsLoggedIn && usrs.Any(ur => ur.UserId == u.Id));
                }
            }

            return retv;
        }
    }
}
