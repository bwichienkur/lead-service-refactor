using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class SalesforceQueryTask
    {
        public string Id { get; set; }
        public string ActivityDateTime__c { get; set; }
        public string LastModifiedDate { get; set; }
        public string CallDisposition { get; set; }
        public string CallDurationInSeconds { get; set; }
        public string CallType { get; set; }
        public string Description { get; set; }
        public string Five9_Call_Recording__c { get; set; }
        public string Five9__Five9AgentName__c { get; set; }
        public string Five9__Five9ANI__c { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
        public string TaskSubtype { get; set; }
    }
}
