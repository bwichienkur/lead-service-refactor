using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDDY.IS.EmsLeadEngine.Entities
{
    public class ISLeadCreateRequest : Request
    {
        public List<int> ISLeadIds { get; set; }
    }
}
