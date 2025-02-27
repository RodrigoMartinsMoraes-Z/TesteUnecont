using Microsoft.EntityFrameworkCore;

using System;

using TesteUnecont.Context.Entities;
using TesteUnecont.Context.TypeConfiguration;

namespace TesteUnecont.Context
{
    public class Context : DbContext
    {
        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.ApplyConfiguration(new LogEntryTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
