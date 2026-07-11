
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{

    [Table("FB.LeadConversionAPIEvent")]
    public partial class LeadConversionAPIEvent
    {
        public int LeadConversionAPIEventId { get; set; }
        public long? LeadId { get; set; }
        public long? EMSLeadId { get; set; }
        public int ConversionAPIEventId { get; set; }
        public int ConversionAPIMessageStatusId { get; set; }
        public string Message { get; set; }
        public int MessageAttemptCount { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsEnabled { get; set; }
    }
}
