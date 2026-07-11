namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Institution")]
    public partial class Institution
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Institution()
        {
            Leads = new HashSet<Lead>();
        }

        public int InstitutionId { get; set; }

        public int? ClientRelationshipId { get; set; }

        [StringLength(18)]
        public string SalesforceId { get; set; }

        [Required]
        [StringLength(255)]
        public string InstitutionName { get; set; }

        [StringLength(100)]
        public string InstitutionShortName { get; set; }

        [StringLength(128)]
        public string InstitutionApproverId { get; set; }

        public bool IsEnabled { get; set; }

        public bool LeadActivityImport { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
        public bool IsPostUpEnabled { get; set; }
        public bool LeadClientInfoImport { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }
        public bool? MigratedMCC { get; set; }
        public bool ProgramStateSyncEnabled { get; set; } = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead> Leads { get; set; }

    }
}
