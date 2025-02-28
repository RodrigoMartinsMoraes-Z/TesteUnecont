using Microsoft.EntityFrameworkCore;

using System;

using TesteUnecont.Api.Entities;

namespace TesteUnecont.Api
{
    public class Context : DbContext
    {
        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.LogGuid).IsRequired();
                entity.Property(e => e.Provider).IsRequired();
                entity.Property(e => e.HttpMethod).IsRequired();
                entity.Property(e => e.StatusCode).IsRequired();
                entity.Property(e => e.UriPath).IsRequired();
                entity.Property(e => e.TimeTaken).IsRequired();
                entity.Property(e => e.ResponseSize).IsRequired();
                entity.Property(e => e.CacheStatus).IsRequired();
                entity.Property(e => e.TimeStamp).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
