using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestApplication.Areas.Identity.Managers;
using TestApplication.Areas.Identity.Models;
using TestApplication.Models.Request;
using TestApplication.Models.RequestJournal;
using TestApplication.Subsystems.Database.Managers;
using TestApplication.Subsystems.SignalR;

namespace TestApplication.Subsystems.RequestProcessing
{
    public class RequestsProcessing
    {
        private List<int> _requestsQueue;
        //Обеспечение безопасной обработки очереди
        private readonly object locker = new object();
        private CancellationTokenSource _requestCTS;
        private CustomUserManager<CustomIdentityUser> _userManager;
        private readonly ILogger<RequestsProcessing> _logger;
        IHubContext<IncomingHub> _signalHub;

        public RequestsProcessing(CustomUserManager<CustomIdentityUser> userManager, ILogger<RequestsProcessing> logger, IHubContext<IncomingHub> signalHub)
        {
            _logger = logger;
            _userManager = userManager;
            _signalHub = signalHub;
            //
            _requestsQueue = new List<int>();
            //Забираем из базы необработанные запросы
            using (RequestManager rm = new RequestManager())
            {
                //очередь внезапного завершения
                rm.InProcess(_userManager, _signalHub);
                //
                var unproc = rm.Unprocessed();
                _requestsQueue.AddRange(unproc);
                _requestsQueue.Sort();
            }
        }

        public Request RegisterRequest(string rid)
        {
            Request retv = null;
            lock (locker)
            {
                using (RequestManager rm = new RequestManager())
                {
                    retv = rm.RegisterRequest(rid);
                }
                if (!_requestsQueue.Contains(retv.Id))
                {
                    _requestsQueue = _requestsQueue.Append(retv.Id).ToList();
                }
                else throw new Exception($"Запрос с номером \"{retv?.Id}\" уже зарегистрирован.");
                return retv;
            }
        }

        public Request AbortRequest(int requestId)
        {
            lock (locker)
            {
                //Может быть в работе у юзера и в массиве его соответственно не будет уже
                if (_requestsQueue.Contains(requestId)) _requestsQueue.Remove(requestId);
                using (RequestManager rm = new RequestManager())
                {
                    return rm.AbortRequest(requestId, _userManager, _signalHub);
                }
            }
        }

        public void RequestError(int requestId)
        {
            lock (locker)
            {
                //Может быть в работе у юзера и в массиве его соответственно не будет уже
                if (_requestsQueue.Contains(requestId)) _requestsQueue.Remove(requestId);
                using (RequestManager rm = new RequestManager())
                {
                    rm.ErrorRequest(requestId, _userManager, _signalHub);
                }
            }
        }

        public void FinalizeRequest(int requestId)
        {
            lock (locker)
            {
                //Подразумевается, что был в обработке и в массиве его быть не должно, 
                //но чтобы например из-за неправильного использования массив не забивался хламом, проверяем
                if (_requestsQueue.Contains(requestId)) _requestsQueue.Remove(requestId);
                using (RequestManager rm = new RequestManager())
                {
                    rm.FinalizeRequest(requestId, _userManager, _signalHub);
                }
            }
        }

        public Request GetRequestState(int requestId)
        {
            using (RequestManager rm = new RequestManager())
            {
                return rm.GetRequestState(requestId);
            }
        }

        public async Task Start()
        {
            _requestCTS = new CancellationTokenSource();
            //Запускаем поток обработки запросов
            RequestProcessorAsync(_requestCTS.Token);
        }

        public void Stop()
        {
            _requestCTS.Cancel();
        }

        private async Task RequestProcessorAsync(CancellationToken cancellationToken)
        {
            int Timeout = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(Timeout);

                    lock (locker)
                    {
                        if (_requestsQueue.Count < 1)
                        {
                            Timeout = 5000;//5 секунд
                            continue;
                        }
                        Timeout = 0;

                        var current = _requestsQueue[0];
                        if (Process(current)) _requestsQueue.RemoveAt(0);
                        else Timeout = 3000;//Ждем свободных юзеров немного, сразу ломиться второй раз бессмысленно
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
            _requestCTS.Dispose();
        }

        private bool Process(int id)
        {
            using (RequestManager rm = new RequestManager())
            {
                return rm.TryAssignRequestToUser(id, _userManager, _signalHub);
            }
        }
    }
}
