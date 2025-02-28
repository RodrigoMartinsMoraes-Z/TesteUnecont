using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TesteUnecont.Api.Entities;

namespace TesteUnecont.Api.Interfaces
{
    public interface ILogEntryRepository
    {
        Task AddLogAsync(LogEntry logEntry);
        Task<LogEntry> GetLogByIdAsync(long id);
        Task<ICollection<LogEntry>> GetLogsByGuidAsync(Guid logGuid);
        Task<ICollection<LogEntry>> GetLogsAsync();
        Task AddRangeLogAsync(IEnumerable<LogEntry> logEntries);
    }
}
