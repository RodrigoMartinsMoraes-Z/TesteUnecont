using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using TesteUnecont.Context.Entities;

namespace TesteUnecont.Api.Services
{
    public class LogConverter
    {
        public string ConvertLogCdnToAgora(string logCdn, DateTime? timeStamp)
        {
            var logs = ExtractLog(logCdn);

            string logAgora = "#Version: 1.0";
            logAgora += NovaLinha($"#Date: {timeStamp ?? DateTime.UtcNow}");
            logAgora +=
                logs
                .Aggregate(
                    NovaLinha(
                        "#Fields: provider http-method status-code uri-path time-taken response-size cache-status"),
                    (accumulator, log) =>
                    accumulator += NovaLinha(
                        $"\"MINHA CDN\" {log.HttpMethod} {log.StatusCode} {log.UriPath} {log.TimeTaken} {log.ResponseSize} {log.CacheStatus}"));

            return logAgora;
        }

        string NovaLinha(string str)
        {
            return $"\r\n{str}";
        }

        private static List<LogEntry> ExtractLog(string log)
        {
            var lines = log.Replace('\"', ' ').Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var logEntries = new List<LogEntry>();

            var logGuid = Guid.NewGuid();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

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

            return logEntries;
        }
    }
}
