using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities
{
    public class Request
    {
        public Guid AuthenticationToken { get; set; }
        public Guid TransactionId { get; set; }
    }
}
