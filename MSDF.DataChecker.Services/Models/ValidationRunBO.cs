using System;
using System.Collections.Generic;
using System.Text;

namespace MSDF.DataChecker.Services.Models
{
  public class ValidationRunBO
    {
        public int Id { get; set; }
        public Guid DatabaseEnvironmentId { get; set; }
        public string RunStatus { get; set; }
        public string HostServer { get; set; }
        public string HostDatabase { get; set; }
        public string Source { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
