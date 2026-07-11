namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeadMarketingAttribution")]
    public partial class LeadMarketingAttribution
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long LeadId { get; set; }

        [StringLength(256)]
        public string Keyword { get; set; }

        [StringLength(256)]
        public string utm_medium { get; set; }

        [StringLength(3000)]
        public string utm_source { get; set; }

        [StringLength(256)]
        public string utm_campaign { get; set; }

        [StringLength(256)]
        public string utm_term { get; set; }

        [StringLength(256)]
        public string utm_content { get; set; }

        [StringLength(256)]
        public string FormName { get; set; }

        [StringLength(255)]
        public string AdGroup { get; set; }

        [StringLength(255)]
        public string SourceCode { get; set; }

        [StringLength(3000)]
        public string CaptureURL { get; set; }
    }
}
