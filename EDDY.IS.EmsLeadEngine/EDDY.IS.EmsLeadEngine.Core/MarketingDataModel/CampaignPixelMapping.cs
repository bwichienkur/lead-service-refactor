namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FB.CampaignPixelMapping")]
    public partial class CampaignPixelMapping
    {
        public int CampaignPixelMappingId { get; set; }
        public Guid CampaignTrackId { get; set; }

        public int PixelId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsEnabled { get; set; }

    }
}
