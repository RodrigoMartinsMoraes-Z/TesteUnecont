using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using TesteUnecont.Context.Entities;
using TesteUnecont.Context.Interfaces;

namespace TesteUnecont.Context.Repository
{
    public class LogEntryRepository : ILogEntryRepository
    {
        private readonly Context _context;

        public LogEntryRepository(Context context)
        {
            _context = context;
        }

        public async Task<LogEntry> GetLogByIdAsync(long id) => await _context.FindAsync<LogEntry>(id).ConfigureAwait(true);

        public async Task<IEnumerable<LogEntry>> GetLogsAsync() => await _context.LogEntries.ToListAsync().ConfigureAwait(true);

        public async Task AddLogAsync(LogEntry logEntry)
        {
            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync().ConfigureAwait(true);
        }
    }
}
