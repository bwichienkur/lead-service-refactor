namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CustomEventInstitutionMapping")]
    public partial class CustomEventInstitutionMapping
    {
        public int CustomEventInstitutionMappingId { get; set; }
        public int EMSInstitutionId { get; set; }
        public int ConversionAPIEventId { get; set; }
        public string FieldType { get; set; }
        public string FieldValue { get; set; }
        public bool FieldUpdatedFromSalesforce { get; set; }
    }
}
