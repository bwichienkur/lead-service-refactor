using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class SalesforceInfoLead
    {
        public bool? InvalidWrongNumberEmailStrategy { get; set; }
        public bool? EmailCommunicationOnly { get; set; }
        public int? StealthAppTotalOutboundDials { get; set; }
        public int? TotalOutboundDialsFromSubStatus { get; set; }
        public string LastDialOutcome { get; set; }
        public int? LastCallAge { get; set; }
        public int? ScheduledSMSCount { get; set; }
        public DateTime? LastCallDate { get; set; }
        public DateTime? NextCallTime { get; set; }
        public DateTime? PreviousCallTime { get; set; }
        public DateTime? NextScheduledSMSDate { get; set; }
        public DateTime? PreviousScheduledSMSDate { get; set; }
    }
}
