using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities
{
    public class Response
    {
        public Guid TransactionId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
