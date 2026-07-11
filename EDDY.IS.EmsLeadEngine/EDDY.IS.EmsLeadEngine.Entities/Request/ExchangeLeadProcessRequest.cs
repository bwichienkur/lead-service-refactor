using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.EmsLeadEngine.Entities.Common;
using static EDDY.IS.EmsLeadEngine.Entities.Constants;

namespace EDDY.IS.EmsLeadEngine.Entities
{
    public class ExchangeLeadProcessRequest 
    {
        public ExchangeLead Lead { get; set; }
        public Dictionary<string, string> AdditionalQuestions { get; set; }
        public ExchangeLeadAction LeadAction { get; set; }
        public ExchangeLeadUniqueKey? LeadUniqueKey { get; set; }
        public Guid SubTransactionId { get; set; }
        public bool RemoveFromFive9 { get; set; }
        public bool? StealthAppIndicator { get; set; }
        public bool? RemoveFromSF { get; set; }
        public bool RouteToActiveCampaign { get; set; } = true;
    }
}
