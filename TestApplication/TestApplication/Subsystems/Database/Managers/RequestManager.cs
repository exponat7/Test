using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApplication.Areas.Identity.Managers;
using TestApplication.Areas.Identity.Models;
using TestApplication.Models.Request;
using TestApplication.Models.RequestJournal;
using TestApplication.Subsystems.SignalR;

namespace TestApplication.Subsystems.Database.Managers
{
    public class RequestManager : BaseManager
    {
        //Обеспечиваем целостную и последовательную обработку каждого запроса
        private static object request_locker = new object();

        public List<Request> GetRequests() => _repo.Requests.Include(r=>r.JournalRecords).OrderByDescending(r => r.Id).ToList();

        public List<Request> GetUserRequests(string userId) => _repo.Requests.Include(r => r.JournalRecords).Where(r => r.UserId == userId).OrderByDescending(r => r.Id).ToList();

        public int GetIncomingRequestsCount()
        {
            int registered = (int)RequestState.Registered;
            int assigned = (int)RequestState.Assigned;
            return _repo.Requests.Count(r => r.LastState == registered || r.LastState == assigned);
        }
        

        public void InProcess(CustomUserManager<CustomIdentityUser> userManager, IHubContext<IncomingHub> hubContext)
        {
            int assigned = (int)RequestState.Assigned;
            List<Request> requests = _repo.Requests.Where(r => r.LastState == assigned).ToList();
            //Для всех стартуем потоки завершения
            Random rand = new Random(DateTime.Now.Millisecond);
            foreach (var request in requests)
            {
                StartFinalization(Startup.Settings.FinalizeRangeFrom, Startup.Settings.FinalizeRangeTo, request.Id, request.UserId, userManager, hubContext,rand);
            }
        }

        public List<int> Unprocessed()
        {
            int registered = (int)RequestState.Registered;
            List<int> retv = _repo.Requests.Where(r => r.LastState == registered).Select(r => r.Id).ToList();
            //
            return retv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="userManager"></param>
        /// <returns>необходимоть постановки в очередь и сам запрос</returns>
        public Request RegisterRequest(string rid)
        {
            Request request = new Request
            {
                IncomingIdentifier = rid,
                State = RequestState.Registered,
            };
            lock (request_locker)//Долбитьcя с отменой можно в любой id(даже случайный) и не хотлось бы, чтобы это получилось прям сразу после создания
            {
                _repo.AddRequest(request);

                RequestsJournalRecord record = new RequestsJournalRecord
                {
                    State = RequestState.Registered,
                    Request = request,
                    StateChanged = DateTime.Now
                };
                using (RequestsJournalManager jm = new RequestsJournalManager())
                {
                    jm.AddRecord(record);
                }
            }
            return request;
        }

        public bool TryAssignRequestToUser(int requestId, CustomUserManager<CustomIdentityUser> userManager, IHubContext<IncomingHub> hubContext)
        {
            Request request = _repo.Requests.First(r => r.Id == requestId);
            lock (request_locker)
            {
                using (RequestsJournalManager jm = new RequestsJournalManager())
                {
                    if (request.State != RequestState.Registered) throw new Exception($"Попытка назначить запрос \"{request.Id}\", хотя он уже в работе или завершен.");
                    var Volunteer = userManager.GetFirstFreeUser(jm.GetRegisterTimeForRequest(request), Startup.Settings.Tm, Startup.Settings.Td,hubContext);
                    if (Volunteer != null)
                    {
                        request.UserId = Volunteer.Id;
                        request.State = RequestState.Assigned;
                        _repo.SaveChanges();
                        //
                        RequestsJournalRecord record = new RequestsJournalRecord
                        {
                            State = RequestState.Assigned,
                            Request = request,
                            StateChanged = DateTime.Now
                        };
                        jm.AddRecord(record);
                        //
                        hubContext.Clients.User(Volunteer.Id).SendAsync("Send",
                        JsonConvert.SerializeObject(new { message = $"Поздравляю, вы обрабатываете запрос № {request.Id}!!!" }));
                        //Стартуем поток завершения
                        Random rand = new Random(DateTime.Now.Millisecond);
                        StartFinalization(Startup.Settings.FinalizeRangeFrom, Startup.Settings.FinalizeRangeTo, request.Id, Volunteer.Id, userManager, hubContext, rand);
                        //
                        return true;
                    }
                }
            }
            return false;
        }

        public Request AbortRequest(int requestId, CustomUserManager<CustomIdentityUser> userManager, IHubContext<IncomingHub> hubContext)
        {
            Request request = _repo.Requests.FirstOrDefault(r => r.Id == requestId);
            if (request == null) return null;
            //После
            lock (request_locker)
            {
                using (RequestsJournalManager jm = new RequestsJournalManager())
                {
                    //var last = jm.FindLastRecordForRequest(request);
                    if (request.State != RequestState.Aborted && request.State != RequestState.Finalized && request.State != RequestState.Error)
                    {
                        request.State = RequestState.Aborted;
                        _repo.SaveChanges();
                        //
                        RequestsJournalRecord record = new RequestsJournalRecord
                        {
                            State = RequestState.Aborted,
                            Request = request,
                            StateChanged = DateTime.Now
                        };
                        jm.AddRecord(record);
                    }
                }
                if (request.UserId != null)
                {
                    userManager.FreeUser(userManager.FindById(request.UserId));
                    hubContext.Clients.User(request.UserId).SendAsync("Send",
                      JsonConvert.SerializeObject(new { message = $"Обработку запроса № {requestId} была отменена." }));
                }
            }
            return request;
        }

        public void FinalizeRequest(int requestId, CustomUserManager<CustomIdentityUser> userManager, IHubContext<IncomingHub> hubContext)
        {
            Request request = _repo.Requests.First(r => r.Id == requestId);
            lock (request_locker)
            {
                using (RequestsJournalManager jm = new RequestsJournalManager())
                {
                    //var last = jm.FindLastRecordForRequest(request);
                    if (request.State != RequestState.Aborted && request.State != RequestState.Finalized && request.State != RequestState.Error)
                    {
                        request.State = RequestState.Finalized;
                        _repo.SaveChanges();
                        //
                        RequestsJournalRecord record = new RequestsJournalRecord
                        {
                            State = RequestState.Finalized,
                            Request = request,
                            StateChanged = DateTime.Now
                        };
                        jm.AddRecord(record);
                    }
                }
                if (request.UserId != null)
                {
                    userManager.FreeUser(userManager.FindById(request.UserId));
                    hubContext.Clients.User(request.UserId).SendAsync("Send",
                      JsonConvert.SerializeObject(new { message = $"Поздравляю, вы завершили обработку запроса № {requestId}!!!" }));
                }
            }
        }

        public void ErrorRequest(int requestId, CustomUserManager<CustomIdentityUser> userManager, IHubContext<IncomingHub> hubContext)
        {
            Request request = _repo.Requests.First(r => r.Id == requestId);
            lock (request_locker)
            {
                using (RequestsJournalManager jm = new RequestsJournalManager())
                {
                    //var last = jm.FindLastRecordForRequest(request);
                    if (request.State != RequestState.Aborted && request.State != RequestState.Finalized && request.State != RequestState.Error)
                    {
                        request.State = RequestState.Error;
                        _repo.SaveChanges();
                        //
                        RequestsJournalRecord record = new RequestsJournalRecord
                        {
                            State = RequestState.Error,
                            Request = request,
                            StateChanged = DateTime.Now
                        };
                        jm.AddRecord(record);
                    }
                }
                if (request.UserId != null)
                {
                    userManager.FreeUser(userManager.FindById(request.UserId));
                    hubContext.Clients.User(request.UserId).SendAsync("Send",
                      JsonConvert.SerializeObject(new { message = $"Обработка запроса № {requestId} завершилась ошибкой" }));
                }
            }
        }

        public Request GetRequestState(int Id)
        {
            return _repo.Requests.FirstOrDefault(r => r.Id == Id);
            //if (request == null) return (null, null);
            //using (RequestsJournalManager jm = new RequestsJournalManager())
            //{
            //    return (request, jm.FindLastRecordForRequest(request).State);
            //}
        }

        public bool HasAssignedRequest(string userId)
        {
            int assigned = (int)RequestState.Assigned;
            return _repo.Requests.Any(r => r.LastState == assigned && r.UserId == userId);
        }

        public Request GetAssignedRequest(string userId)
        {
            int assigned = (int)RequestState.Assigned;
            return _repo.Requests.SingleOrDefault(r => r.LastState == assigned && r.UserId == userId);
        }

        private static async Task StartFinalization(int From, int To, int requestId, string userId, CustomUserManager<CustomIdentityUser> userManager, IHubContext<IncomingHub> hubContext, Random rand)
        {
            var delay = rand.Next(From, To);
            await Task.Delay(delay);
            using (RequestManager rm = new RequestManager())
            {
                rm.FinalizeRequest(requestId, userManager, hubContext);
            }
        }
    }
}