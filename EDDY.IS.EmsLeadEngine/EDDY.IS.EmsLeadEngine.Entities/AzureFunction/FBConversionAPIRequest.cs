using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.AzureFunction
{
    public class FBConversionAPIRequest : BaseAPIRequest
    {
        public long EMSLeadId { get; set; }
        public string CampaignTrackId { get; set; }
        public long? ISLeadId { get; set; }
    }
}
