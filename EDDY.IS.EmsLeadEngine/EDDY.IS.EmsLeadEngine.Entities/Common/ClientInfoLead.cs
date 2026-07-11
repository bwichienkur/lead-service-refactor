using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class ClientInfoLead : ClientInfoLeadBase
    {
        public DateTime? ClientApplicationDate { get; set; }
        public string ClientApplicationDegreeName { get; set; }
        public DateTime? ClientStatusUpdatedDate { get; set; }
        public DateTime? ClientInterviewDate { get; set; }
        public DateTime? ClientStartDate { get; set; }
        public string CustomFields { get; set; }
        public DateTime? ClientStartReceivedDate { get; set; }
        public DateTime? ClientContactDate { get; set; }
        public DateTime? ClientEnrollDate { get; set; }
        public DateTime? ClientApplicationStartDate { get; set; }
        public DateTime? ClientAdmitDate { get; set; }
        public DateTime? ClientAppointmentDate { get; set; }
        public DateTime? ClientQualifiedDate { get; set; }
        public DateTime? ClientGraduateDate { get; set; }
        public DateTime? ClientFirstTermPersistDate { get; set; }
        public DateTime? ClientFAFSAReceivedDate { get; set; }
        public string ClientRegistered { get; set; }
        public DateTime? ClientApplicationSubmittedDate { get; set; }
        public DateTime? ClientDepositDate { get; set; }
        public DateTime? ClientApplicationDeniedDate { get; set; }

        //Inherited 3 fields (PendingApplicationChecklistItems, CompletedApplicationChecklistItems, PipelineNotes ) from ClientInfoLeadBase
    }
}
