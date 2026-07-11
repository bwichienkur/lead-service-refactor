using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.AzureFunction
{
    public class BaseLeadRequest
    {
        public long EMSLeadId { get; set; }
        public string DatabaseServer { get; set; }
        public Guid TransactionId { get; set; }
        public Guid? SubTransactionId { get; set; }
        public bool UseSalesforceSandbox { get; set; }
        public bool RemoveFromFive9 { get; set; }
        public Constants.LeadServiceRequestType RequestType { get; set; }
    }
}
