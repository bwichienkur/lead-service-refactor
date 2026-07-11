namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeadTransaction")]
    public partial class LeadTransaction
    {
        public long LeadTransactionId { get; set; }

        public Guid TransactionId { get; set; }

        public Guid? SubTransactionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Action { get; set; }

        public long? LeadId { get; set; }

        public bool Success { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
