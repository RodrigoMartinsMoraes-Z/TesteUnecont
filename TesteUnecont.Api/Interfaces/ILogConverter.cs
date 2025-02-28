using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TesteUnecont.Api.Entities;

namespace TesteUnecont.Api.Interfaces
{
    public interface ILogConverter
    {
        Task<string> ConvertLogCdnToAgora(string logCdn, DateTime? timeStamp);

        Task<string> ConvertLogToAgora(List<LogEntry> logs, DateTime? timeStamp);

        Task<string> ConvertLogToCdn(List<LogEntry> logs);

        Task<List<LogEntry>> ExtractLog(string log);
    }
}
