using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class LegacyGPLead : ClientInfoLeadBase
    {
        public int? ProgramProductId { get; set; }
        public int? LeadStatusId { get; set; }
        public int? LeadSubStatusId { get; set; }
        public int? LeadStateId { get; set; }
        public string LegacyGPLeadId { get; set; }
        public Guid TransactionId { get; set; }
        public int? InstitutionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string LeadOwner { get; set; }
        public string PreferredMethodOfContact { get; set; }
    }
}
