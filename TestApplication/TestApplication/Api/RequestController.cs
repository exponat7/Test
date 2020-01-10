using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TestApplication.Common;
using TestApplication.Subsystems.SignalR;

namespace TestApplication.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly ILogger<RequestController> _logger;
        //private readonly IHubContext<IncomingHub> _hubContext;

        public RequestController(ILogger<RequestController> logger/*, IHubContext<IncomingHub> hubContext*/)
        {
            _logger = logger;
            //_hubContext = hubContext;
        }

        /// <summary>
        /// Регистрация входящего запроса
        /// </summary>
        /// <param name="identifier"></param>
        [HttpPost]
        public IActionResult PostRequest([FromBody]string identifier)
        {
            try
            {
                var Request = Startup.Processing.RegisterRequest(identifier);
                return Ok(Request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получение состояния запроса
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetRequestState(int id)
        {
            try
            {
                var request = Startup.Processing.GetRequestState(id);
                if (request == null) return NotFound(id);
                return Ok(request.State.GetDisplayName());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Отмена запроса клиентом
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult CancellRequest(int id)
        {
            try
            {
                var request = Startup.Processing.AbortRequest(id);
                if (request == null) return NotFound(id);
                return Ok(request.State.GetDisplayName());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Мониторинг состояния сервера
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok("Ok");
        }
    }
}