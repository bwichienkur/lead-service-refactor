using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    [Table("LeadSalesforceInfo")]
    public class LeadSalesforceInfo
    {
        public int LeadSalesforceInfoId { get; set; }
        public long LeadId { get; set; }
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
