using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class SalesforceQueryNote
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LastModifiedDate { get; set; }
        public string CreatedDate { get; set; }
    }

    public class ContentDocumentLink
    {
        public string Id { get; set; }
        public string ContentDocumentId { get; set; }
    }
}
