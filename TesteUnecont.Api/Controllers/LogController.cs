using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TesteUnecont.Api.Entities;
using TesteUnecont.Api.Interfaces;

namespace TesteUnecont.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogConverter _logConverter;
        private readonly ILogEntryRepository _logEntryRepository;

        public LogController(ILogConverter logConverter, ILogEntryRepository logEntryRepository)
        {
            _logConverter = logConverter;
            _logEntryRepository = logEntryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var logs = await _logEntryRepository.GetLogsAsync().ConfigureAwait(true);
            if (!logs.Any())
            {
                return NoContent();
            }
            return Ok(logs);
        }

        [HttpGet]
        [Route("{guid}")]
        public async Task<IActionResult> GetByGuidAsync(Guid guid)
        {
            var logs = _logEntryRepository.GetLogsByGuidAsync(guid).Result.ToList();
            if (!logs.Any())
            {
                return NoContent();
            }

            var logCnd = await _logConverter.ConvertLogToCdn(logs);
            var logAgora = await _logConverter.ConvertLogToAgora(logs, DateTime.UtcNow);

            return Ok($"{logCnd}\r\n{logAgora}");
        }

        [HttpGet]
        [Route("logcnd")]
        public async Task<IActionResult> GetCdnByGuidAsync(Guid guid)
        {
            var logs = _logEntryRepository.GetLogsByGuidAsync(guid).Result.ToList();
            if (!logs.Any())
            {
                return NoContent();
            }

            var logCnd = await _logConverter.ConvertLogToCdn(logs);

            return Ok($"{logCnd}");
        }

        [HttpGet]
        [Route("logagora")]
        public async Task<IActionResult> GetAgoraByGuidAsync(Guid guid)
        {
            var logs = _logEntryRepository.GetLogsByGuidAsync(guid).Result.ToList();
            if (!logs.Any())
            {
                return NoContent();
            }

            var logAgora = await _logConverter.ConvertLogToAgora(logs, DateTime.UtcNow);

            return Ok($"{logAgora}");
        }

        [HttpPost]
        [Route("convert/{logCdn}")]
        public async Task<IActionResult> ConvertAsync(string logCdn)
        {
            if (string.IsNullOrEmpty(logCdn))
            {
                return BadRequest("O log CDN não pode ser nulo ou vazio.");
            }

            try
            {
                var logEntry = await _logConverter.ExtractLog(logCdn);

                if (!logEntry.Any())
                {
                    return BadRequest("O log CDN não contém entradas válidas.");
                }

                var logAgora = await _logConverter.ConvertLogToAgora(logEntry, DateTime.UtcNow);

                return Ok(logAgora);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar o log: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("save/{logCdn}")]
        public async Task<IActionResult> ConvertSaveAsync(string logCdn)
        {
            if (string.IsNullOrEmpty(logCdn))
            {
                return BadRequest("O log CDN não pode ser nulo ou vazio.");
            }

            try
            {
                var logEntry = await _logConverter.ExtractLog(logCdn);

                if (!logEntry.Any())
                {
                    return BadRequest("O log CDN não contém entradas válidas.");
                }

                var logAgora = await _logConverter.ConvertLogToAgora(logEntry, DateTime.UtcNow);

                await _logEntryRepository.AddRangeLogAsync(logEntry).ConfigureAwait(true);
                return Ok(logAgora);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar o log: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("save/file/{logCdn}")]
        public async Task<IActionResult> SaveAsync(string logCdn)
        {
            if (string.IsNullOrEmpty(logCdn))
            {
                return BadRequest("O log CDN não pode ser nulo ou vazio.");
            }

            try
            {
                var logEntry = await _logConverter.ExtractLog(logCdn);

                if (!logEntry.Any())
                {
                    return BadRequest("O log CDN não contém entradas válidas.");
                }

                var logAgora = _logConverter.ConvertLogToAgora(logEntry, DateTime.UtcNow);

                string path = $".\\{logEntry.FirstOrDefault().LogGuid}.txt";

                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.WriteLine(logAgora);
                }

                return Ok(path);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar o log: {ex.Message}");
            }
        }

    }
}
