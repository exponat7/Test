using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApplication.Areas.Identity.Managers;
using TestApplication.Areas.Identity.Models;

namespace TestApplication.Areas.Identity.Initialization
{
    /// <summary>
    /// Класс инициализации приложения, создание записей по умолчанию
    /// </summary>
    public class InitialDefaults
    {
        /// <summary>
        /// Роль администратора(менедженр будет администратором)
        /// </summary>
        private const string adminRoleName = "manager";
        /// <summary>
        /// Логин администратора
        /// </summary>
        private const string adminName = "manager";
        /// <summary>
        /// Пароль администратора
        /// </summary>
        private const string adminPassword = "12345678aA";
        /// <summary>
        /// Роль "Оператор"
        /// </summary>
        private const string operatorRoleName = "operator";

        /// <summary>
        /// Роль "Директор"
        /// </summary>
        private const string directorRoleName = "director";
        /// <summary>
        /// Логин директора
        /// </summary>
        private const string directorName = "director";
        /// <summary>
        /// Пароль директора
        /// </summary>
        private const string directorPassword = "12345678aA";

        /// <summary>
        /// Метода регистрации в системе администратора при запуске приложения
        /// </summary>
        /// <returns></returns>
        public static async Task CreateIfNotExistAsync(CustomUserManager<CustomIdentityUser> _userManager, RoleManager<IdentityRole> _roleManager, string name, string roleName, string password)
        {
            //Проверяем на первый запуск(существование хотя бы одного пользователя)
            if (!(await _userManager.GetUsersInRoleAsync(roleName)).Any())
            {
                //Перед назначением роли , нужно убедиться, что есть что назначать
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    //Создаем роль
                    await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                }
                //Проверяем нет ли  польователя
                var user = await _userManager.FindByNameAsync(name);
                if (user == null)
                {
                    //Если такого юзера нет, создаем дефолтного админа
                    user = new CustomIdentityUser { UserName = name/*, Email = Input.Email*/ };
                    /*var result = */await _userManager.CreateAsync(user, password);
                }
                //Теперь назначаем роль, если она еще не назначена
                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    //Правим юзера
                    await _userManager.AddToRoleAsync(user, roleName);
                }
                //Активируем
                await _userManager.SetIsActiveAsync(user, true);
            }
            //Тут активировать админа не стоит по причине того, что уже может быть создан не дефолтный админ(с целями повышения безопасности)
        }
        
        /// <summary>
        /// Метод создания роли "Оператор"
        /// </summary>
        /// <param name="_userManager">Менеджер учетных записей</param>
        /// <param name="_roleManager">Менеджер паролей</param>
        /// <returns></returns>
        public static async Task CreateRoleIfNotExistAsync(CustomUserManager<CustomIdentityUser> _userManager, RoleManager<IdentityRole> _roleManager, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
            }
            //Проверяем всех юзеров, если роли не назначено, то назначаем оператора(может возникнуть при удалении руками из базы роли оператора)
            var UsersWithoutRoles = _userManager.Users.ToList();
            foreach (var user in UsersWithoutRoles)
            {
                if (!(await _userManager.GetRolesAsync(user)).Any())
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
        /// <summary>
        /// Метод инициализации при запуске приложения
        /// </summary>
        /// <param name="_userManager">Менеджер учетных записей</param>
        /// <param name="_roleManager">Менеджер паролей</param>
        /// <returns></returns>
        public static async Task InitDefaults(CustomUserManager<CustomIdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            //Создаем первого админа, чтобы мог назначать роли и т.д.
            await CreateIfNotExistAsync(_userManager, _roleManager, adminName, adminRoleName, adminPassword);
            //Так как полноценной админки у нас не будет, директора также хардкодим
            await CreateIfNotExistAsync(_userManager, _roleManager, directorName, directorRoleName, directorPassword);
            //
            await CreateRoleIfNotExistAsync(_userManager, _roleManager, operatorRoleName);
        }
    }
}
