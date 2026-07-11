namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LeadSubStatus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LeadSubStatus()
        {
            Leads = new HashSet<Lead>();
            LeadStatusHistories = new HashSet<LeadStatusHistory>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LeadSubStatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string LeadSubStatusName { get; set; }

        public int LeadStatusId { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreatedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead> Leads { get; set; }

        public virtual LeadStatus LeadStatus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeadStatusHistory> LeadStatusHistories { get; set; }
    }
}
