namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeadHistory")]
    public partial class LeadHistory
    {
        public long LeadHistoryId { get; set; }

        public long LeadId { get; set; }

        [Required]
        public string JsonData { get; set; }

        public DateTime Timestamp { get; set; }

        public short LeadSourceTypeId { get; set; }

        public virtual Lead Lead { get; set; }
    }
}
