using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class SalesforceApplication
    {
        public bool? PersonalStatementReceived { get; set; }
        public bool? LOR1 { get; set; }
        public bool? LOR2 { get; set; }
        public bool? LOR3 { get; set; }
        public bool? ResumeReceived { get; set; }
        public string AppStartTerm { get; set; }
    }
}
