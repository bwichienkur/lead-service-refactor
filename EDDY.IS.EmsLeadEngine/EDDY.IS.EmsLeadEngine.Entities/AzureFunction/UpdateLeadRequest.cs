using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.AzureFunction
{
    public class UpdateLeadRequest : BaseLeadRequest
    {
        public bool? RouteToFive9 { get; set; }
        public bool Upsert { get; set; }
    }
}
