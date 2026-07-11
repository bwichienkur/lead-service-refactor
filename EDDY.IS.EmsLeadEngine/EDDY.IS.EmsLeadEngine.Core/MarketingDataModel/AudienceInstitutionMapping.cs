
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    [Table("FB.AudienceInstitutionMapping")]
    public class AudienceInstitutionMapping
    {
        public int AudienceInstitutionMappingId { get; set; }
        public int AudienceId { get; set; }
        public int InstitutionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsEnabled { get; set; }
    }
}
