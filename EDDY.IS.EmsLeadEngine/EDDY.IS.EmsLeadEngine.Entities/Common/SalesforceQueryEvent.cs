using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class SalesforceQueryEvent
    {
        public string Id { get; set; }
        public string ActivityDateTime__c { get; set; }
        public string LastModifiedDate { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string EventSubtype { get; set; }
    }
}
