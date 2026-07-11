namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("dataex.VW_ProgramMapping")]
    public partial class VW_ProgramMapping
    {
        public int InstitutionId { get; set; }
        [Key]
        public int ProgramProductId { get; set; }
        public string ClientDegreeName { get; set; }
        public int CampusTypeId { get; set; }
        public bool IsHybrid { get; set; }
        public string ClientCampusName { get; set; }
        public string ProgramName { get; set; }
        public int ProgramId { get; set; }
        public int CampusId { get; set; }
        public int ProgramLevelId { get; set; }
        public bool DoNotRouteToSF { get; set; }
        public bool RouteToActiveCampaign { get; set; }
    }
}
