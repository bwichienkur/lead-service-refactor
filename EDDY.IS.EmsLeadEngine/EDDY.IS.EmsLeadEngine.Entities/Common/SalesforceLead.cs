using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class SalesforceLead : LeadBase
    {
        public string SalesforceId { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentSubStatus { get; set; }
        public string CurrentState { get; set; }
        public string LeadOwner { get; set; }       
        public string InstitutionSalesforceId { get; set; }
        public string ProgramProductSalesforceId { get; set; }
        public string LegacyGPLeadId { get; set; }
        public string CurrentClientStatus { get; set; }
        public bool IsStealthApp { get; set; }
        public string RewarmingIndicator { get; set; }
        public string AppDialOutcome { get; set; }
        public string RewarmingDialOutcome { get; set; }
        public double? RewarmingContactAttempts { get; set; }
        public bool HasOptedOutOfSMS { get; set; }
        public string Timezone { get; set; }
        public string TimezoneOffset { get; set; }
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
        public string PendingApplicationChecklistItems { get; set; }
        public string CompletedApplicationChecklistItems { get; set; }
        public string PipelineNotes { get; set; }

    }
}
