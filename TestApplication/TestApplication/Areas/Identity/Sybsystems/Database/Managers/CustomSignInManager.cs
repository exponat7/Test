using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Features;
using System.Linq;
using System.Net;
using TestApplication.Areas.Identity.Managers;
using TestApplication.Areas.Identity.Models;
using Microsoft.AspNetCore.SignalR;
using TestApplication.Subsystems.SignalR;
using Newtonsoft.Json;
using TestApplication.Models.ViewModels;

namespace TestApplication.Areas.Identity.Managers
{
    /// <summary>
    /// Переопределение менеджера входа в приложение
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomSignInManager<T> : SignInManager<T> where T : class
    {
        /// <summary>
        /// Менеджер учетной записи пользователя
        /// </summary>
        private CustomUserManager<T> _userManager;

        private IHttpContextAccessor _httpContextAccessor;

        private IHubContext<IncomingHub> _hubContext;

        //id юзера + user ip(могут залогиниться под одной учеткой с разных хостов)
        private static List<(string, IPAddress)> _usersLoggedIn = new List<(string, IPAddress)>();

        /// <summary>
        /// Список ошибок
        /// </summary>
        private List<string> _errors;

        /// <summary>
        /// Конструктор
        /// </summary>
        public CustomSignInManager(CustomUserManager<T> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<T> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor, ILogger<CustomSignInManager<T>> logger, IAuthenticationSchemeProvider schemes, IHttpContextAccessor httpContextAccessor,
            IHubContext<IncomingHub> hubContext)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Переопределение метода проверки пароля при входе в приложение
        /// </summary>        
        public override async Task<SignInResult> PasswordSignInAsync(T user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            _errors = new List<string>();
            var result = await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
            if (result.Succeeded)
            {
                //Пускаем только активные учетки
                if (!_userManager.GetIsActive(user))
                {
                    await SignOutAsync();
                    _errors.Add("Учетная запись не активирована. Для активации учетной записи обратитесь к администратору системы");
                    return SignInResult.Failed;
                }
                if (user is CustomIdentityUser User)
                {
                    //
                    IPAddress ua = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
                    _usersLoggedIn.Add((User.Id, ua));
                    if(User.IsFree)
                    {
                        _hubContext.Clients.All.SendAsync("UserUpdate",
                                        JsonConvert.SerializeObject(new { id = User.Id, state = (int)UserState.Free }));
                    }
                    else
                    {
                        _hubContext.Clients.All.SendAsync("UserUpdate",
                                        JsonConvert.SerializeObject(new { id = User.Id, state = (int)UserState.Busy }));
                    }
                    await _userManager.SetIsLoggedInAsync(user, true);
                    //await _userManager.SetIsFreeAsync(user, true);
                    //
                }
            }
            //
            return result;
        }
        /// <summary>
        /// Метод выхода пользователя из аккаунта
        /// </summary>
        /// <returns></returns>
        public override async Task SignOutAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user is CustomIdentityUser User)
            {
                IPAddress ua = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
                _usersLoggedIn.RemoveAll(u=>u.Item1==User.Id&&u.Item2.Equals(ua));
                await _userManager.SetIsLoggedInAsync(user, false);
                _hubContext.Clients.All.SendAsync("UserUpdate",
                                        JsonConvert.SerializeObject(new { id = User.Id, state = (int)UserState.NotLoggedIn }));
                //await _userManager.SetIsFreeAsync(user, false);
            }
            //
            await base.SignOutAsync();
        }

        /// <summary>
        /// Переопределение метода отслеживания состояния пользователя
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public override bool IsSignedIn(ClaimsPrincipal principal)
        {
            bool retv = base.IsSignedIn(principal);
            var user = _userManager.GetUserAsync(principal).Result;
            var _userID = _userManager.GetUserId(principal);
            IPAddress ua = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            if (retv)
            {
                if (!_usersLoggedIn.Contains((_userID, ua)))
                {
                    SignOutAsync().Wait();//С разных машин логиниться нельзя
                    //_usersLoggedIn.Add((_userID, ua));
                }
                _userManager.SetIsLoggedInAsync(user, true).Wait();
            }
            else
            {
                if (_usersLoggedIn.Contains((_userID, ua))) _usersLoggedIn.RemoveAll(u => u.Item1 == _userID && u.Item2.Equals(ua));
                _userManager.SetIsLoggedInAsync(user, false).Wait();
            }
            return retv;
        }
        
        /// <summary>
        /// Метод возврата списка ошибок при входе в приложение
        /// </summary>
        /// <returns></returns>
        public List<string> GetErrors()
        {
            return _errors ?? new List<string>();
        }

        /// <summary>
        /// Проверка статуса текущего пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsUserLoggedIn(CustomIdentityUser user)
        {
            IPAddress ua = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            return _usersLoggedIn.Any(u => u.Item1 == user.Id && !u.Item2.Equals(ua));
        }

        /// <summary>
        /// Проверка подключения пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsUserSignedIn(CustomIdentityUser user) => _usersLoggedIn.Any(u => u.Item1 == user.Id);

        /// <summary>
        /// Получение числа пользовалей, считающихся подключенными
        /// </summary>
        /// <returns></returns>
        public static int GetLoggedInUsersCount()
        {
            //Distinct нужен когда пользователь с одной машины на
            return _usersLoggedIn.Distinct().Count();
        }

        internal async Task DeleteUserFromLoggedIn(T user)
        {
            if (user is CustomIdentityUser User) _usersLoggedIn.RemoveAll(u => u.Item1 == User.Id);
            await _userManager.SetIsLoggedInAsync(user, false);
        }
    }
}