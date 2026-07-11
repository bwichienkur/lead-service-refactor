using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.EmsLeadEngine.Entities.Common;
namespace EDDY.IS.EmsLeadEngine.Entities
{
    public class LegacyGPBulkLeadSaveRequest : Request
    {
        public List<LegacyGPLead> Leads { get; set; }
    }
}
