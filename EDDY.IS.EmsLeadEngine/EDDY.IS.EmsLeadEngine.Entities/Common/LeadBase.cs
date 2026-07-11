using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class LeadBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
        public int? EducationLevelId { get; set; }
        public string StartTerm { get; set; }
        public string Notes { get; set; }
        public string ClosedReasonCode { get; set; }
        public bool IsTest { get; set; }
        public string ExternalId { get; set; }
        public bool IsActive { get; set; }
        public bool HasOptedOutOfEmail { get; set; }
        public Guid? LeadGUID { get; set; }
        public string UTM_Campaign { get; set; }
        public string UTM_Channel { get; set; }
        public string UTM_Vendor { get; set; }
    }
}
