using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tester.Models;

namespace Tester
{
    class Program
    {
        private static string _log_path;
        private static string _app_path;
        private static ConfigModel _config;
        private static string _target_path;
        private static SemaphoreSlim _locker;
        //
        private delegate Task CallFinishedSuccess(string message);

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">0 - minimal delay(ms), 1 - maximal delay(ms), 2 - requests count</param>
        static async Task Main(string[] args)
        {
            _locker = new SemaphoreSlim(1);
            //Получаем текущую директорию приложения
            _app_path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _log_path = Path.Combine(_app_path, "log", "log.txt");
            //Парсим входные параметры(из аргументов, если их там нет, то берем из конфигурационного файла)
            _config = new ConfigModel();
            int? minDel, maxDel, reqC;
            (minDel, maxDel, reqC) = await TryParseArgsAsync(args);
            //Парсим остальное из конфига
            _config = await TryParseConfigAsync();
            //Аргументы в приоритете
            _config.MinDelay = minDel ?? _config.MinDelay;
            _config.MaxDelay = maxDel ?? _config.MaxDelay;
            _config.RequestsCount = reqC ?? _config.RequestsCount;
            //Если никто ничего не задал берем настройки по умолчанию
            _config.MinDelay ??= 10;
            _config.MaxDelay ??= 120;
            _config.Port ??= 5000;
            _config.MaxWaitForAnswerTimeoutMs ??= 350000;//5 мин
            //
            //Регистрируем клиента(на текушей машине!!!)
            _target_path = $"http://localhost:{_config.Port}/api/Request/";
            //Запускаем цикл отправки запросов
            int? count = _config.RequestsCount;
            Random rand = new Random(DateTime.Now.Millisecond);//Чтобы даже при каждом запуске были разные значения задаем seed
            int minDelay = _config.MinDelay.Value * 1000;//в милисекундах
            int maxDelay = _config.MaxDelay.Value * 1000;
            //если перепутать местами, то ArgumentOutOfRangeException //rand.Next(minDelay, maxDelay);
            if (minDelay > maxDelay)
            {
                var tmp = minDelay;
                minDelay = maxDelay;
                maxDelay = tmp;
            }
            while (true)
            {
                if (count.HasValue)
                {
                    if (count.Value > 0) count--;
                    else break;
                }
                //Расчитываем текущую случайную задержку перед отправкой запроса
                int curDelay = rand.Next(minDelay, maxDelay);
                await Task.Delay(curDelay);
                //
                var result = await SendClientIncomingRequestAsync($"206-206-{DateTime.Now}");
                if (result.Item1)
                {
                    Console.WriteLine($"Запрос с номером \"{result.Item2}\" успешно зарегистрирован");
                    WaitForRequestFinished(result.Item2, Success);
                }
            }
        }

        private static async Task Success(string message)
        {
            Console.WriteLine(message);
            //await LogAsync("Запрос успешно завершен");
        }

        private static async Task WaitForRequestFinished(int requestId, CallFinishedSuccess success)
        {
            HttpClient client = new HttpClient();
            try
            {
                string prevState = "Новый";
                while (true)
                {
                    CancellationTokenSource cts = new CancellationTokenSource(2000);//Ставим 2 секунды
                    try
                    {
                        var response = await client.GetAsync(new Uri($"{_target_path}{requestId}"), cts.Token);
                        string state = await response.Content.ReadAsStringAsync();
                        if (!state.Equals(prevState, StringComparison.OrdinalIgnoreCase))
                        {
                            prevState = state;
                            if (state.Equals("Обработан", StringComparison.OrdinalIgnoreCase) ||
                                state.Equals("Прерван", StringComparison.OrdinalIgnoreCase) ||
                                state.Equals("Ошибка", StringComparison.OrdinalIgnoreCase))
                            {
                                await success($"Запрос с номером \"{requestId}\" завершен с состоянием \"{state}\".");
                                break;
                            }
                            else await success($"Запрос с номером \"{requestId}\" перешел в состояние \"{state}\".");
                        }
                        await Task.Delay(1000);//через секунду снова опрашиваем статус
                    }
                    catch (OperationCanceledException)
                    {
                        if (!await Ping())
                        {
                            cts?.Dispose();
                            throw new HttpRequestException("Сервис недоступен.");
                        }
                        //throw new HttpRequestException("Превышен интервал ожидания ответа от сервиса."); 
                    }
                    finally { cts?.Dispose(); }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await LogAsync(ex.ToString());
            }
            finally
            {
                client?.Dispose();
            }
        }

        /// <summary>
        /// Проверка состояния сервиса
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> Ping()
        {
            CancellationTokenSource cts = new CancellationTokenSource(2000);//Ставим 2 секунды
            HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetAsync(new Uri($"{_target_path}ping"), cts.Token);
                string Ok = await response.Content.ReadAsStringAsync();
                return Ok.Equals("Ok", StringComparison.OrdinalIgnoreCase);
            }
            //timeout
            catch (OperationCanceledException) { return false; }
            //непредвиденная ошибка
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await LogAsync(ex.ToString());
                return false;
            }
            finally
            {
                cts?.Dispose();
                client?.Dispose();
            }
        }

        /// <summary>
        /// Отсылаем запрос на входящий звонок
        /// </summary>
        /// <returns>статус звонка и его идентификатор(guid)</returns>
        private static async Task<(bool, int)> SendClientIncomingRequestAsync(string RequestNumber)
        {
            //using (CancellationTokenSource cts = new CancellationTokenSource(_config.MaxWaitForAnswerTimeoutMs.Value))
            //using (HttpClient client = new HttpClient())
            //{
            CancellationTokenSource cts = new CancellationTokenSource(_config.MaxWaitForAnswerTimeoutMs.Value);
            HttpClient client = new HttpClient();
            try
            {
                var response = await client.PostAsync(new Uri($"{_target_path}"), new StringContent(JsonConvert.SerializeObject(RequestNumber), Encoding.UTF8, MediaTypeNames.Application.Json), cts.Token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string requestId = await response.Content.ReadAsStringAsync();
                    return (true, int.Parse(requestId));
                }
                return (false, 0);
            }
            //timeout
            catch (OperationCanceledException) { return (false, 0); }
            //непредвиденная ошибка
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await LogAsync(ex.ToString());
                return (false, 0);
            }
            finally
            {
                cts?.Dispose();
                client?.Dispose();
            }
            //}
        }

        private static async Task<(int?, int?, int?)> TryParseArgsAsync(string[] args)
        {
            (int?, int?, int?) retv = (null, null, null);
            try
            {
                if (args.Length > 0) retv.Item1 = int.Parse(args[0]);
                if (args.Length > 1) retv.Item2 = int.Parse(args[1]);
                if (args.Length > 2) retv.Item3 = int.Parse(args[2]);
            }
            catch (Exception ex)
            {
                await LogAsync(ex.ToString());
            }
            return retv;
        }

        private static async Task<ConfigModel> TryParseConfigAsync()
        {
            ConfigModel retv = null;
            try
            {
                string json = await File.ReadAllTextAsync(Path.Combine(_app_path, "appsettings.json"));
                retv = JsonConvert.DeserializeObject<ConfigModel>(json);
            }
            catch (Exception ex)
            {
                await LogAsync(ex.ToString());
            }
            return retv;
        }

        private static async Task LogAsync(string message)
        {
            try
            {
                //Чтобы в файл все писалось последовательно из разных потоков
                await _locker.WaitAsync();
                //
                Directory.CreateDirectory(Path.GetDirectoryName(_log_path));
                await File.AppendAllTextAsync(_log_path, $"{DateTime.Now}:{Environment.NewLine}{message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally { _locker.Release(); }
        }
    }
}
