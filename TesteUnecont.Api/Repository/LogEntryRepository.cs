using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TesteUnecont.Api.Entities;
using TesteUnecont.Api.Interfaces;

namespace TesteUnecont.Api.Repository
{
    public class LogEntryRepository : ILogEntryRepository
    {
        private readonly Context _context;

        public LogEntryRepository(Context context)
        {
            _context = context;
        }

        public async Task<LogEntry> GetLogByIdAsync(long id) => await _context.FindAsync<LogEntry>(id).ConfigureAwait(true);

        public async Task<ICollection<LogEntry>> GetLogsAsync() => await _context.LogEntries.ToListAsync().ConfigureAwait(true);

        public async Task<ICollection<LogEntry>> GetLogsByGuidAsync(Guid logGuid) => await _context.LogEntries.Where(x => x.LogGuid == logGuid).ToListAsync().ConfigureAwait(true);

        public async Task AddLogAsync(LogEntry logEntry)
        {
            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync().ConfigureAwait(true);
        }

        public async Task AddRangeLogAsync(IEnumerable<LogEntry> logEntries)
        {
            _context.LogEntries.AddRange(logEntries);
            await _context.SaveChangesAsync().ConfigureAwait(true);
        }
    }
}
