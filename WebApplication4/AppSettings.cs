using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication4
{
    public class AppSettings
    {
        public string StudentApiBaseUrl { get; set; }
        public string SSRSReportUrl { get; set; }
        public string SSRSUsername { get; set; }
        public string SSRSPassword { get; set; }
        public string SSRSDomain { get; set; }
        public string ExportFilePath { get; set; }
    }
}
