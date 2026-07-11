using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    [Table("Google.AudienceLeadEvent")]
    public class GAudienceLeadEvent
    {
        [Key]
        public int AudienceLeadEventId { get; set; }
        public long? LeadId { get; set; }
        public long? EMSLeadId { get; set; }
        public int? AudienceId { get; set; }
        public int AudienceEventId { get; set; }
        public int AudienceMessageStatusId { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public int MessageAttemptCount { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsEnabled { get; set; }
    }
}
