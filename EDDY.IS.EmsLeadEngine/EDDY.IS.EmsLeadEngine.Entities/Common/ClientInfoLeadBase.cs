using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class ClientInfoLeadBase: LeadBase
    {
        public string ClientInitialStartTerm { get; set; }
        public string ClientNotes { get; set; }
        public string ClientStatus { get; set; }
        public string ClientApplicationStartTerm { get; set; }
        public string PipelineNotes { get; set; } //UMO-32 : Mount Olive SF to EMS sync
        public string PendingApplicationChecklistItems { get; set; } //umo-26
        public string CompletedApplicationChecklistItems { get; set; }
    }
}
