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

        public async Task<List<LogEntry>> ExtractLog(string log)
        {
            var parts = log.Replace('\"', ' ').Replace("%2F", string.Empty).Replace(Environment.NewLine, " ").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var logEntries = new List<LogEntry>();
            var logGuid = Guid.NewGuid();

            for (int i = 0; i < parts.Length; i += 4)
            {
                if (i + 4 >= parts.Length)
                    break; // Certifica-se de que há partes suficientes para formar uma entrada completa

                // Verifica se a última parte contém números seguidos pelo início de uma nova linha
                string timeTakenPart = parts[i + 4];
                string nextResponseSizePart = string.Empty;

                // Verifica se a parte 'timeTakenPart' contém espaço seguido do próximo valor
                if (timeTakenPart.Contains(' '))
                {
                    var timeTakenParts = timeTakenPart.Split(' ');
                    timeTakenPart = timeTakenParts[0];
                    nextResponseSizePart = timeTakenParts[1];
                }

                var logEntry = new LogEntry();
                logEntry.Provider = "MINHA CDN";
                logEntry.ResponseSize = int.Parse(parts[i].Trim());
                logEntry.StatusCode = int.Parse(parts[i + 1].Trim());
                logEntry.CacheStatus = parts[i + 2].Trim() == "INVALIDATE" ? "REFRESH_HIT" : parts[i + 2].Trim();
                logEntry.HttpMethod = parts[i + 3].Trim().Split(' ')[0];
                logEntry.UriPath = parts[i + 3].Trim().Split(' ')[1].Trim();
                logEntry.TimeTaken = (int)Math.Round(
                       decimal.Parse(timeTakenPart.Trim(), CultureInfo.InvariantCulture));
                logEntry.LogGuid = logGuid;
                logEntry.TimeStamp = DateTime.UtcNow;

                logEntries.Add(logEntry);

                // Ajusta o índice para lidar com a parte dividida
                if (!string.IsNullOrEmpty(nextResponseSizePart))
                {
                    parts[i + 4] = nextResponseSizePart;
                }
            }

            return logEntries;
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
