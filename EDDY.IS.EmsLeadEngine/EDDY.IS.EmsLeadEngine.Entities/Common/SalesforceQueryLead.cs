using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class SalesforceQueryLead
    {
        public const String SObjectTypeName = "Lead";
        public const string CompanyName = "EducationDynamics";

        public String Id { get; set; }

        public string Company = CompanyName;
        /// <summary>
        /// Institution sfid
        /// </summary>
        public String Account__c { get; set; }

        public string Client_Status__c { get; set; }

        /// <summary>
        /// ProgramID sfid
        /// </summary>
        public string Degree__c { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        public string Education_Level__c { get; set; }
        public String Email { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }

        public String Status { get; set; }
        public String Lead_Sub_Status__c { get; set; }

        public string Lead_State__c { get; set; }

        /// <summary>
        /// Picklist
        /// </summary>
        public string LeadSource { get; set; }
        public String Other_Phone__c { get; set; }

        /// <summary>
        /// LeadOwner --> school Group to be assigned sfid
        /// </summary>
        public SalesforceQueryUser Assigned_Advisor__r { get; set; }
        
        public String Phone { get; set; }

        /// <summary>
        /// TBD should be a picklist
        /// </summary>
        public SalesforceQueryStartTerm Start_Term__r { get; set; }
        public string Utm_Campaign__c { get; set; }
        public string Utm_Channel__c { get; set; }
        public string Utm_Vendor__c { get; set; }

        public string Notes__c { get; set; }

        public bool RouteToFive9__c { get; set; }

        public bool RemoveFromFive9__c { get; set; }

        public bool From_API__c { get; set; }

        public bool IsTest__c { get; set; }

        public bool HasOptedOutOfEmail { get; set; }

        public string LeadGUID__c { get; set; }

        public string Closed_Reason_Code__c { get; set; }

        public string ExternalIdentifier__c { get; set; }
        public bool Stealth_App__c { get; set; }
        public string RewarmingIndicator__c { get; set; }
        public string App_Dial_Outcome__c { get; set; }
        public string Rewarming_Dial_Outcome__c { get; set; }
        public double? Rewarming_Contact_Attempts__c { get; set; }
        public bool tdc_tsw__SMS_Opt_out__c { get; set; }
        public bool MCC_Invalid_Wrong_Email_Strategy__c { get; set; }
        public bool MCC_Email_Communication_Only__c { get; set; }
        public string MCC_StealthApp_Total_Outbound_Dials__c { get; set; }
        public string MCC_Total_Outbound_Dials_from_Sub_Status__c { get; set; }

        public string Last_Dial_Outcome__c { get; set; }
        public string Lead_Last_Dial_Age__c { get; set; }
        public string Automated_SMS_Count__c { get; set; }
        public DateTime? Last_Dial_Date__c { get; set; }
        public DateTime? Next_Dial_Time__c { get; set; }
        public DateTime? Previous_Dial_Time__c { get; set; }
        public DateTime? Next_SMS_Date__c { get; set; }
        public DateTime? Previous_SMS_Date__c { get; set; }

        public string Pipeline_Notes__c { get; set; }
    }
}
