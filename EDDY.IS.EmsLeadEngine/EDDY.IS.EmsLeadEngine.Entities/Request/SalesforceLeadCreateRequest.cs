using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.EmsLeadEngine.Entities.Common;

namespace EDDY.IS.EmsLeadEngine.Entities
{
    public class SalesforceLeadCreateRequest : Request
    {
        public SalesforceLead Lead { get; set; }
        public Dictionary<string, string> AdditionalQuestions { get; set; }
    }
}
