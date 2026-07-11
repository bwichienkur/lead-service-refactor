namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeadActivity")]
    public partial class LeadActivity
    {
        public long LeadActivityId { get; set; }

        public long LeadId { get; set; }

        public int LeadActivityTypeId { get; set; }

        public long LeadStatusHistoryId { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(30)]
        public string SalesforceLastModifiedDate { get; set; }

        [Required]
        public string JsonData { get; set; }

        public virtual Lead Lead { get; set; }

        public virtual LeadActivityType LeadActivityType { get; set; }

        public virtual LeadStatusHistory LeadStatusHistory { get; set; }
    }
}
