using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities
{
    public class Constants
    {
        public const string ERR_AUTHENTICATION = "Authentication Token is not valid.";
        public const string ERR_GENERAL = "Error: {0}.";
        public const string OK_GENERAL = "OK";
        public const string OK_WARNING = "OK, {0}";
        public const string STATUS_NEW = "New Lead";
        public const int    STATUS_NEW_ID = 1;
        public const int STATE_OPEN_ID = 1;
        public const int STATE_CLOSED_ID = 2;
        public const string STATE_CLOSEDREASON_CODE = "Duplicate";
        public const string STATE_OPEN = "Open";
        public const int    DUPLICATE_DAYSPAN = 30;
        public const string EMSTRANSACTION_INSERT = "INSERT";
        public const string EMSTRANSACTION_UPDATE = "UPDATE";
        public const string EMSTRANSACTION_UPSERT = "UPSERT";
        public const string TEST_LEAD_EMAIL_STRING = "@test";
        public const string TEST_LEAD = "Test Lead";
        public const string CLOSED_REASON_CODE_MARKETINGONLY = "Marketing Lead Only";
        public const string CLOSED_REASON_CODE_DUPLICATE = "Duplicate";
        public const string CLOSED_REASON_CODE_MARKETINGONLYSCREENEDLEAD = "Marketing Lead Only - Screened Lead";
        public const string LEAD_PROGRAM_FORMAT_ONLINE = "Online";
        public const string LEAD_PROGRAM_FORMAT_GROUND = "On Campus";
        public const string CLOSED_REASON_CODE_SCHOOLWORKINGLEAD = "School Working Lead";
        public const int MCC_STATUS_NEW_ID = 17;
        public const int MCC_SUBSTATUS_NEW_ID = 57;


        public enum ResponseCode
        {
            AUTHENTICATION_ERROR = -2,
            ERROR = -1,
            OK = 0,
            WARNING = 1
        }

        public enum LeadSourceType
        {
            LandingPage = 1,
            DataExchange = 2,
            SalesForce = 3,
            GPImport = 4,
            Microsite = 5
        }

        public enum LeadServiceRequestType
        {
            LandingPage = 1,
            DataExchange = 2,
            Salesforce = 3,
            GPImport = 4,
            Unsubscribe = 5,
            EmailStrategyExhausted = 6
        }

        public enum ExchangeLeadAction
        {
            Insert = 1,
            Update = 2,
            Upsert = 3
        }

        public enum ExchangeLeadUniqueKey
        {
            ExternalId = 1,
            LeadGUID = 2,
            LegacyGPLeadId = 3,
            EmailAddress = 4,
            EmailProgram = 5,
            ISLeadId = 6,
            Phone1 = 7,
            EMSLeadId = 8,
            FirstLastName = 9,
            NameAndEmailorPhone = 10
        }

        public enum LeadActivityType
        {
            Task = 1,
            Email = 2,
            Event = 3,
            Note = 4
        }

        public enum LeadHistoryAction
        {
            Create = 1,
            Update = 2
        }

        public enum CampusType
        { 
            Online = 1,
            Ground = 2
        }

        public enum ConversionAPIEvent
        {
            Lead = 1,
            Applied = 2,
            Started = 3
        }

        public enum FBConversionAPIEvent
        {
            Lead = 1,
            Applied = 2,
            Started = 3
        }

        public enum FBAudienceEvent
        {
            Lead = 1,
            Applied = 2,
            Started = 3
        }

        public enum GoogleConversionAPIEvent
        {
            Lead = 1,
            Applied = 2,
            Started = 3
        }

        public enum Educationlevel
        { 
            HaventCompletedHighSchool = 1
        }

        public enum ProgramStateSyncRuleType
        {
            ALLOW_ONLY = 1,
            ALLOW_ALL,
            ALLOW_ALL_EXCEPT,
            ALLOW_NONE
        }        
    }
}
