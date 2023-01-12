using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MSDF.DataChecker.Persistence.RuleExecutionLogs;

namespace MSDF.DataChecker.Persistence.ValidationsRun
{
    [Table("ValidationRun", Schema = "datachecker")]
    public class ValidationRun
    {
        public int Id { get; set; }
        public string RunStatus { get; set; }
        public string HostServer { get; set; }
        public string HostDatabase { get; set; }
        public string Source { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
