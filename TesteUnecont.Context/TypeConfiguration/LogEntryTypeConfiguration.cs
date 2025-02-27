using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Text;

using TesteUnecont.Context.Entities;

namespace TesteUnecont.Context.TypeConfiguration
{
    public class LogEntryTypeConfiguration : IEntityTypeConfiguration<LogEntry>
    {
        public void Configure(EntityTypeBuilder<LogEntry> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
