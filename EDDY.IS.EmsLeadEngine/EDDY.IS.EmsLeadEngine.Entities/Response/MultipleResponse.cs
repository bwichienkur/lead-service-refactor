using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities
{
    public class MultipleResponse
    {
        public List<Response> Responses { get; set; }

        public Guid TransactionId { get; set; }

        public MultipleResponse()
        {
            Responses = new List<Response>();
        }
    }
}
