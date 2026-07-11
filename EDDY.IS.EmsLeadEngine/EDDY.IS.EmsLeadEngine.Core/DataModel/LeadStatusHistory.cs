namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeadStatusHistory")]
    public partial class LeadStatusHistory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LeadStatusHistory()
        {
            LeadActivities = new HashSet<LeadActivity>();
        }

        public long LeadStatusHistoryId { get; set; }

        public long LeadId { get; set; }

        public int LeadStatusId { get; set; }

        public int? LeadSubStatusId { get; set; }

        [StringLength(100)]
        public string ClosedReasonCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? LeadStateId { get; set; }
        public string LeadOwner { get; set; }

        public virtual Lead Lead { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeadActivity> LeadActivities { get; set; }

        public virtual LeadStatus LeadStatus { get; set; }

        public virtual LeadSubStatus LeadSubStatus { get; set; }
    }
}
