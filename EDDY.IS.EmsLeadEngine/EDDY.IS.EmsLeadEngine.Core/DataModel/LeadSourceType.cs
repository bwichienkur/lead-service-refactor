namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeadSourceType")]
    public partial class LeadSourceType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LeadSourceType()
        {
            Leads = new HashSet<Lead>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LeadSourceTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string LeadSourceTypeName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreatedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead> Leads { get; set; }
    }
}
