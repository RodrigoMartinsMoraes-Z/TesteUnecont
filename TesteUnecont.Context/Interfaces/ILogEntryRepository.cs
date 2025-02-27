using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TesteUnecont.Context.Entities;

namespace TesteUnecont.Context.Interfaces
{
    public interface ILogEntryRepository
    {
        Task AddLogAsync(LogEntry logEntry);
        Task<LogEntry> GetLogByIdAsync(long id);
        Task<IEnumerable<LogEntry>> GetLogsAsync();
    }
}
