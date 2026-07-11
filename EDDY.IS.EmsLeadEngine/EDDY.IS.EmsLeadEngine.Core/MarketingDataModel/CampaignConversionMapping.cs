using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Google.CampaignConversionMapping")]
    public partial class CampaignConversionMapping
    {
        public int CampaignConversionMappingId { get; set; }
        public Guid CampaignTrackId { get; set; }

        public int ConversionId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsEnabled { get; set; }

    }
}
