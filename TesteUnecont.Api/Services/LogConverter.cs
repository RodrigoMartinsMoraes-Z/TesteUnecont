using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using TesteUnecont.Api.Entities;
using TesteUnecont.Api.Interfaces;

namespace TesteUnecont.Api.Services
{
    public class LogConverter : ILogConverter
    {
        public async Task<string> ConvertLogCdnToAgora(string logCdn, DateTime? timeStamp)
        {
            var logs = await ExtractLog(logCdn);

            return await ConvertLogToAgora(logs, timeStamp);
        }

        public Task<string> ConvertLogToAgora(List<LogEntry> logs, DateTime? timeStamp)
        {
            string logAgora = "#Version: 1.0";
            logAgora += NovaLinha($"#Date: {timeStamp?.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) ?? DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");
            logAgora +=
                logs
                .Aggregate(
                    NovaLinha("#Fields: provider http-method status-code uri-path time-taken response-size cache-status"),
                    (accumulator, log) =>
                    accumulator += NovaLinha(
                        $"\"MINHA CDN\" {log.HttpMethod} {log.StatusCode} {log.UriPath} {log.TimeTaken} {log.ResponseSize} {log.CacheStatus}"));

            return Task.FromResult(logAgora);
        }

        public Task<string> ConvertLogToCdn(List<LogEntry> logs)
        {
            string logCnd = string.Empty;

            foreach (var log in logs)
            {
                if (string.IsNullOrWhiteSpace(logCnd))
                    logCnd = $"{log.ResponseSize}|{log.StatusCode}|{ConvertCacheStatus(log.CacheStatus)}|\"{log.HttpMethod} {log.UriPath} HTTP/1.1\"|{log.TimeTaken.ToString(CultureInfo.InvariantCulture)}";
                else
                    logCnd += NovaLinha(
                        $"{log.ResponseSize}|{log.StatusCode}|{ConvertCacheStatus(log.CacheStatus)}|\"{log.HttpMethod} {log.UriPath} HTTP/1.1\"|{log.TimeTaken.ToString(CultureInfo.InvariantCulture)}");
            }

            return Task.FromResult(logCnd);
        }

        string NovaLinha(string str)
        {
            return $"\r\n{str}";
        }

        public Task<List<LogEntry>> ExtractLog(string log)
        {
            var lines = log.Replace('\"', ' ').Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var logEntries = new List<LogEntry>();

            var logGuid = Guid.NewGuid();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length < 5) continue;

                var logEntry = new LogEntry
                {
                    Provider = "MINHA CDN",
                    HttpMethod = parts[3].Trim().Split(' ')[0],
                    StatusCode = int.Parse(parts[1].Trim()),
                    UriPath = parts[3].Trim().Split(' ')[1].Trim(),
                    TimeTaken = (int)Math.Round(decimal.Parse(parts[4].Trim(), CultureInfo.InvariantCulture)),
                    ResponseSize = int.Parse(parts[0]),
                    CacheStatus = parts[2].Trim() == "INVALIDATE" ? "REFRESH_HIT" : parts[2].Trim(),
                    LogGuid = logGuid,
                    TimeStamp = DateTime.UtcNow
                };

                logEntries.Add(logEntry);
            }

            return Task.FromResult(logEntries);
        }

        private string ConvertCacheStatus(string status)
        {
            if (status == "HIT")
                return "HIT";
            else if (status == "MISS")
                return "MISS";
            else if (status == "REFRESH_HIT")
                return "INVALIDATE";
            else
                return status;
        }

    }
}
