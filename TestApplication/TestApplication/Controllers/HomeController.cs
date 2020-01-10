using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApplication.Areas.Identity.Managers;
using TestApplication.Areas.Identity.Models;
using TestApplication.Models;
using TestApplication.Models.Request;
using TestApplication.Models.RequestJournal;
using TestApplication.Models.ViewModels;
using TestApplication.Subsystems.Database.Managers;

namespace TestApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        /// <summary>
        /// Менеджер учетных записей пользователей
        /// </summary>
        private readonly CustomUserManager<CustomIdentityUser> _userManager;

        public HomeController(CustomUserManager<CustomIdentityUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            //Получаем статус юзера(обрабатывает ли он запрос)
            Request userRequest = null;
            using (RequestManager rm = new RequestManager())
            {
                userRequest = rm.GetAssignedRequest(user.Id);
            }
            if (userRequest != null) ViewBag.AssignedId = userRequest.Id;

            MainModel model = new MainModel();

            model.StartTab.Requests = await PrepareIncoming();

            return View(model);
        }

        /// <summary>
        /// Обновление диалога настроек
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateSettingsDialog()
        {
            SettingsModel model = new SettingsModel
            {
                Tm = Startup.Settings.Tm/1000,
                Td = Startup.Settings.Td/1000,
                From = Startup.Settings.FinalizeRangeFrom/1000,
                To = Startup.Settings.FinalizeRangeTo/1000
            };
            return PartialView("_SettingsView", model);
        }
        /// <summary>
        /// Сохранение настроек
        /// </summary>
        /// <param name="model">Модель с данными</param>
        /// <returns></returns>
        public IActionResult SaveSettings(SettingsModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_SettingsView", model);
            }
            Startup.Settings.Save(model.Tm.Value*1000, model.Td.Value*1000, model.From.Value*1000, model.To.Value*1000);
            return Json(new { success = true });
        }

        public async Task<IActionResult> SelectTab(int Id)
        {
            switch (Id)
            {
                case 0:
                    return await IncomingTab();
                case 1:
                    return await UsersTab();
                case 2:
                    return await AdminTab();
            }
            return null;//500
        }

        public async Task<IActionResult> IncomingTab()
        {
            var model = await PrepareIncoming();
            return PartialView("_IncomingTabView", model);
        }

        public async Task<IActionResult> UsersTab()
        {
            UsersModel model = new UsersModel();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                UserState state = UserState.NotLoggedIn;
                if (user.IsLoggedIn && user.IsFree) state = UserState.Free;
                if (user.IsLoggedIn && !user.IsFree) state = UserState.Busy;
                model.Users.Add(new SingleUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    CurrentState = state
                });
            }

            return PartialView("_UsersTabView", model);
        }

        public async Task<IActionResult> AdminTab()
        {
            return PartialView("_AdminTabView");
        }

        private async Task<IncomingRequests> PrepareIncoming()
        {
            bool isManager = await _userManager.IsInRoleAsync(User, "manager");
            bool isDirector = await _userManager.IsInRoleAsync(User, "director");
            IncomingRequests model = new IncomingRequests();
            List<Request> Requests = null;
            using (RequestManager rm = new RequestManager())
            {
                model.IncomingCount = rm.GetIncomingRequestsCount();
                if (isDirector || isManager)
                {
                    Requests = rm.GetRequests();
                }
                else
                {
                    var user = await _userManager.GetUserAsync(User);
                    Requests = rm.GetUserRequests(user.Id);
                }
            }
            //
            foreach (var request in Requests)
            {
                var record = new JournalRecord();
                if (!string.IsNullOrEmpty(request.UserId)) record.UserName = _userManager.FindById(request.UserId).UserName;
                record.Id = request.Id;
                record.IncomingIdentifier = request.IncomingIdentifier;
                record.LastState = request.State;
                record.RegisterTime = request.JournalRecords.First(jr => jr.State == RequestState.Registered).StateChanged;
                record.StartTime = request.JournalRecords.FirstOrDefault(jr => jr.State == RequestState.Assigned)?.StateChanged;
                record.EndTime = request.JournalRecords.OrderBy(jr => jr.Id).FirstOrDefault(jr => jr.State == RequestState.Aborted||
                jr.State == RequestState.Error || jr.State == RequestState.Finalized)?.StateChanged;
                //
                model.Records.Add(record);
            }

            return model;
        }
    }
}