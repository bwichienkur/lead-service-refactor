using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    [Table("Google.AudienceInstitutionMapping")]
    public class GAudienceInstitutionMapping
    {
        [Key]
        public int AudienceInstitutionMappingId { get; set; }
        public int AudienceId { get; set; }
        public int InstitutionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsEnabled { get; set; }
        public bool UseMultipleFilterTypes { get; set; }
        public int SequenceNumber { get; set; }
    }
}
