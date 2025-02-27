using System;

namespace TesteUnecont.Context.Entities
{
    public class LogEntry
    {
        public long Id { get; set; }
        public Guid LogGuid { get; set; }
        public string Provider { get; set; }
        public string HttpMethod { get; set; }
        public int StatusCode { get; set; }
        public string UriPath { get; set; }
        public int TimeTaken { get; set; }
        public int ResponseSize { get; set; }
        public string CacheStatus { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
