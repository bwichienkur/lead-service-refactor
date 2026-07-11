namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeadState")]
    public partial class LeadState
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LeadState()
        {
            Leads = new HashSet<Lead>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LeadStateId { get; set; }

        [Required]
        [StringLength(50)]
        public string LeadStateName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreatedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead> Leads { get; set; }
    }
}
