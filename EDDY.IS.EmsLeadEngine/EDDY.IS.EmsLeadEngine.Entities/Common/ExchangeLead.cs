using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class ExchangeLead : ClientInfoLead
    {
        public int? LeadStatusId { get; set; }
        public int? LeadSubStatusId { get; set; }
        public int? LeadStateId { get; set; }
        public string LegacyGPLeadId { get; set; }
        public int EMSInstitutionId { get; set; }
        public int? ProgramProductId { get; set; }
        public int? ISLeadId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string lma_utm_medium { get; set; }
        public string lma_utm_source { get; set; }
        public string lma_utm_term { get; set; }
        public string lma_utm_campaign { get; set; }
        public string lma_utm_content { get; set; }
        public string lma_keyword { get; set; }
        public string lma_adgroup { get; set; }
        public string lma_captureurl { get; set; }
        public int? MediaPlanItemId { get; set; }
    }
}
