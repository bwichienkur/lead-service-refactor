using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    [Table("LeadClientInfo")]
    public class LeadClientInfo
    {
        public long LeadClientInfoId { get; set; }

        public long LeadId { get; set; }

        [StringLength(200)]
        public string Status { get; set; }

        public DateTime? ApplicationDate { get; set; }

        [StringLength(200)]
        public string InitialStartTerm { get; set; }

        [StringLength(200)]
        public string ApplicationStartTerm { get; set; }

        [StringLength(200)]
        public string ApplicationDegreeName { get; set; }

        public string Notes { get; set; }
        
        public virtual Lead Leads { get; set; }
        public DateTime? StatusUpdatedDate { get; set; }
        public DateTime? InterviewDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string CustomFields { get; set; }
        public DateTime? StartReceivedDate { get; set; }
        public DateTime? ContactDate { get; set; }
        public DateTime? EnrollDate { get; set; }
        public DateTime? ApplicationStartDate { get; set; }
        public DateTime? AdmitDate { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime? QualifiedDate { get; set; }
        public DateTime? GraduateDate { get; set; }
        public DateTime? FirstTermPersistDate { get; set; }
        public DateTime? FAFSAReceivedDate { get; set; }
        public string Registered { get; set; }
        public DateTime? ApplicationSubmittedDate { get; set; }
        public DateTime? DepositDate { get; set; }
        public DateTime? ApplicationDeniedDate { get; set; }
        public string PendingApplicationChecklistItems { get; set; }
        public string CompletedApplicationChecklistItems { get; set; }
        public string PipelineNotes { get; set; }
    }
}
