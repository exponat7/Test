using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TestApplication.Areas.Identity.Models;
using TestApplication.Areas.Identity.Subsystems.Database.Repository;
using TestApplication.Models.ViewModels;
using TestApplication.Subsystems.Database.Managers;
using TestApplication.Subsystems.SignalR;

namespace TestApplication.Areas.Identity.Managers
{
    /// <summary>
    /// Переопределение менеджера учетной записи
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomUserManager<T> : UserManager<T> where T : class
    {
        /// <summary>
        /// Репозиторий подсистемы Identity
        /// </summary>
        private readonly IdentityRepository _repo;

        /// <summary>
        /// Конструктор
        /// </summary>
        public CustomUserManager(IUserStore<T> store, IOptions<IdentityOptions> options, IPasswordHasher<T> passwordHasher,
            IEnumerable<IUserValidator<T>> userValidators, IEnumerable<IPasswordValidator<T>> passwordValidators,
            ILookupNormalizer lookupNormalizer, IdentityErrorDescriber describer, IServiceProvider serviceProvider, ILogger<UserManager<T>> logger)
            : base(store, options, passwordHasher, userValidators, passwordValidators, lookupNormalizer, describer, serviceProvider, logger)
        {
            _repo = new IdentityRepository();
            Logger = logger;
        }

        public CustomIdentityUser FindById(string Id) => _repo.GetUserById(Id);

        /// <summary>
        /// Метод изменения состояния учетной записи пользователя
        /// </summary>
        /// <param name="user">Контекст пользователя</param>
        /// <param name="IsActive">Состояние учетной записи</param>
        /// <returns></returns>
        public async Task<IdentityResult> SetIsActiveAsync(T user, bool IsActive)
        {
            try
            {
                var type = user.GetType();
                if (type == typeof(CustomIdentityUser))
                {
                    _repo.UpdateIsActiveStatus(user as CustomIdentityUser, IsActive);
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
            return IdentityResult.Success;
        }

        /// <summary>
        /// Метод изменения состояния учетной записи пользователя
        /// </summary>
        /// <param name="user">Контекст пользователя</param>
        /// <param name="IsActive">Состояние учетной записи</param>
        /// <returns></returns>
        public async Task<IdentityResult> SetIsFreeAsync(T user, bool IsFree)
        {
            try
            {
                var type = user.GetType();
                if (type == typeof(CustomIdentityUser))
                {
                    _repo.UpdateIsFreeStatus(user as CustomIdentityUser, IsFree);
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
            return IdentityResult.Success;
        }

        /// <summary>
        /// Метод изменения состояния учетной записи пользователя
        /// </summary>
        /// <param name="user">Контекст пользователя</param>
        /// <param name="IsActive">Состояние учетной записи</param>
        /// <returns></returns>
        public async Task<IdentityResult> SetIsLoggedInAsync(T user, bool IsLoggedIn)
        {
            try
            {
                var type = user.GetType();
                if (type == typeof(CustomIdentityUser))
                {
                    _repo.UpdateIsLoggedInStatus(user as CustomIdentityUser, IsLoggedIn);
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
            return IdentityResult.Success;
        }

        //статический локер поможет решить проблему конкуренции между назначением пользователя и отменой запроса,
        //так же при должной проверке мы не получим на одного пользователя более одного запроса,
        private object _locker = new object();

        public CustomIdentityUser GetFirstFreeUser(DateTime CallStart, int Tm, int Td, IHubContext<IncomingHub> hubContext)
        {
            lock (_locker)
            {
                //актуализируем статусы всех пользователей(могут теоретически сбиться при внезапном отключении например)
                var users = _repo.GetAllUsers();
                using (RequestManager rm = new RequestManager())
                {
                    foreach (var user in users)
                    {
                        if (rm.HasAssignedRequest(user.Id))
                        {
                            if (user.IsFree)
                            {
                                _repo.UpdateIsFreeStatus(user, false);
                                if (user.IsLoggedIn)
                                {
                                    hubContext.Clients.All.SendAsync("UserUpdate",
                                        JsonConvert.SerializeObject(new { id = user.Id, state = (int)UserState.Busy }));
                                }
                                else
                                {
                                    hubContext.Clients.All.SendAsync("UserUpdate",
                                        JsonConvert.SerializeObject(new { id = user.Id, state = (int)UserState.NotLoggedIn }));
                                }
                            }
                        }
                        else
                        {
                            if (!user.IsFree)
                            {
                                _repo.UpdateIsFreeStatus(user, true);
                                if (user.IsLoggedIn)
                                {
                                    hubContext.Clients.All.SendAsync("UserUpdate",
                                        JsonConvert.SerializeObject(new { id = user.Id, state = (int)UserState.Free }));
                                }
                                else
                                {
                                    hubContext.Clients.All.SendAsync("UserUpdate",
                                        JsonConvert.SerializeObject(new { id = user.Id, state = (int)UserState.NotLoggedIn }));
                                }
                            }
                        }
                    }
                }
                //while (true)
                {
                    var retv = _repo.GetFirstFreeUser(CallStart, Tm, Td);
                    if (retv != null)
                    {
                        _repo.UpdateIsFreeStatus(retv, false);
                        hubContext.Clients.All.SendAsync("UserUpdate",
                                        JsonConvert.SerializeObject(new { id = retv.Id, state = (int)UserState.Busy }));
                        ////Проверяем наличие необработанных запросов
                        //using (RequestManager rm = new RequestManager())
                        //{
                        //    if (rm.HasAssignedRequest(retv.Id))
                        //    {
                        //        continue;
                        //    }
                        //}
                    }
                    return retv;
                }
            }
        }

        public void FreeUser(CustomIdentityUser user)
        {
            lock (_locker)
            {
                _repo.UpdateIsFreeStatus(user, true);
            }
        }

        /// <summary>
        /// Метод получения состояния учетной записи пользователя
        /// </summary>
        /// <param name="user">Контекст учетной записи пользователя</param>        
        public bool GetIsActive(T user)
        {
            if (user == null) return false;
            return (bool)user.GetType().GetProperty(nameof(CustomIdentityUser.IsActive))?.GetValue(user);
        }

        /// <summary>
        /// Метод получения признака активности по ClaimsPrincipal
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task<bool> GetIsActiveAsync(ClaimsPrincipal principal)
        {
            return GetIsActive(await GetUserAsync(principal));
        }

        /// <summary>
        /// Метод проверки роли учетной записи 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> IsInRoleAsync(ClaimsPrincipal principal, string role)
        {
            T User = await GetUserAsync(principal);
            if (User == null) return false;
            return await IsInRoleAsync(User, role);
        }
    }
}