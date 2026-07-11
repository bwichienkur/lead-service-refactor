using EDDY.IS.EmsLeadEngine.Core.DataModel;
using EDDY.IS.EmsLeadEngine.Core.Extensions;
using EDDY.IS.EmsLeadEngine.Core.Properties;
using EDDY.IS.EmsLeadEngine.Entities.Common;
using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public class SalesforceProcessor
    {
        const string SALESFORCE_AUTH_SANDBOX = "https://test.salesforce.com/services/oauth2/token";
        const string SALESFORCE_AUTH = "https://login.salesforce.com/services/oauth2/token";
        
        private static ForceClient forceClient;
        

        private ForceClient AuthenticateUser(Guid transactionId, bool forceReconnect = false)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var auth = new AuthenticationClient();
            var url = Settings.Default.UseSalesforceSandbox ? SALESFORCE_AUTH_SANDBOX : SALESFORCE_AUTH;

            auth.UsernamePasswordAsync(EnvironmentSettings.SalesForceConsumerKey
                    , EnvironmentSettings.SalesForceConsumerSecret
                    , EnvironmentSettings.SalesForceUsername
                    , EnvironmentSettings.SalesforcePassword, url).Wait();


            LogManager.LogJournalInfo(transactionId, $"Connected to Salesforce Sandbox = {Settings.Default.UseSalesforceSandbox}");

            if (forceClient == null)
            {
                forceClient = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
            }
            else if (forceReconnect)
            {
                lock (forceClient)
                {
                    forceClient = null;
                    forceClient = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
                }
            }
            
            return forceClient;
        }
        
        public SalesforceLead GetLead(string leadId, Guid transactionId, bool forceReconnect = false)
        {

            SalesforceQueryLead sfQueryLead = null;

            try
            {
                // Authenticate with Salesforce
                var client = AuthenticateUser(transactionId, forceReconnect);
                //LeadId not provided on upsert operation
                sfQueryLead = client.QueryAsync<SalesforceQueryLead>($"SELECT id, Status, Lead_Sub_Status__c, Lead_State__c, Assigned_Advisor__r.Name, FirstName, LastName, Email, Phone ,Other_Phone__c, Street, City, State, Country, PostalCode, Education_Level__c, Start_Term__r.Name, Account__c, Degree__c, Notes__c, HasOptedOutOfEmail, LeadGUID__c, Closed_Reason_Code__c, IsTest__c, Client_Status__c,Stealth_App__c,RewarmingIndicator__c,App_Dial_Outcome__c,Rewarming_Dial_Outcome__c,Rewarming_Contact_Attempts__c,tdc_tsw__SMS_Opt_out__c,Utm_Channel__c, MCC_Invalid_Wrong_Email_Strategy__c, MCC_Email_Communication_Only__c, MCC_StealthApp_Total_Outbound_Dials__c, MCC_Total_Outbound_Dials_from_Sub_Status__c, Last_Dial_Outcome__c, Lead_Last_Dial_Age__c, Automated_SMS_Count__c, Last_Dial_Date__c, Next_Dial_Time__c, Previous_Dial_Time__c, Next_SMS_Date__c, Previous_SMS_Date__c, Pipeline_Notes__c  FROM Lead WHERE id ='{leadId}'").Result?.Records?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (!forceReconnect && 
                    (ex.Message.ToLowerInvariant().Contains("expired") || ex.InnerException.Message.ToLowerInvariant().Contains("expired")))
                {
                    LogManager.LogJournalException(transactionId, new Exception($"Salesforce session expired re-connecting", ex));
                    return GetLead(leadId, transactionId, true);
                }
                else
                {
                    throw ex;
                }
            }

            if (sfQueryLead == null)
            {
                throw new Exception($"Could not retrieve lead data from Salesforce, SalesforceId={leadId}");
            }
            
            return MapSalesforceLeadFromSalesforceQueryLead(sfQueryLead);
        }

        /// <summary>
        /// Gets the list of tasks (Five9 Task or Email) for a lead after a given date
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="lastModifiedDate"></param>
        /// <param name="transactionId"></param>
        /// <param name="forceReconnect"></param>
        /// <returns></returns>
        public List<SalesforceQueryTask> GetTasks(string leadId, string lastModifiedDate, bool emailTask, Guid transactionId, bool forceReconnect = false)
        {

            List<SalesforceQueryTask> Result = new List<SalesforceQueryTask>();
            lastModifiedDate = lastModifiedDate ?? "2001-01-01T12:00:00.000Z";

            try
            {
                // Authenticate with Salesforce
                var client = AuthenticateUser(transactionId, forceReconnect);

                string subTaskQuery = string.Empty;

                if (emailTask)
                {
                    subTaskQuery = " TaskSubtype = 'Email' ";
                }
                else
                {
                    subTaskQuery = " TaskSubtype = 'Task' ";
                }

                //Tasks and e-mails
                Result = client.QueryAsync<SalesforceQueryTask>($" SELECT Id, ActivityDateTime__c, LastModifiedDate, CallDisposition, CallDurationInSeconds, CallType, Description, Five9_Call_Recording__c, Five9__Five9AgentName__c, Five9__Five9ANI__c, Status, Subject, TaskSubtype " +
                                                                $" FROM Task " +
                                                                $" WHERE Who.Type = 'Lead' AND {subTaskQuery} AND Who.Id = '{leadId}' " + 
                                                                $" AND  LastModifiedDate > {lastModifiedDate}" +
                                                                $" ORDER BY LastModifiedDate ASC NULLS FIRST ").Result?.Records;


                Result = Result ?? new List<SalesforceQueryTask>();
                LogManager.LogJournalInfo(transactionId, $"Salesforce GetTasks (EmailTask={emailTask}) from Salesforce for lead {leadId}", Result); 

            }
            catch (Exception ex)
            {
                if (!forceReconnect &&
                    (ex.Message.ToLowerInvariant().Contains("expired") || ex.InnerException.Message.ToLowerInvariant().Contains("expired")))
                {
                    LogManager.LogJournalException(transactionId, new Exception($"Salesforce session expired re-connecting", ex));
                    return GetTasks(leadId, lastModifiedDate, emailTask, transactionId, true);
                }
                else
                {
                    throw ex;
                }
            }


            return Result;
        }

        /// <summary>
        /// Get the list of events for a lead after a given date
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="lastModifiedDate"></param>
        /// <param name="transactionId"></param>
        /// <param name="forceReconnect"></param>
        /// <returns></returns>
        public List<SalesforceQueryEvent> GetEvents(string leadId, string lastModifiedDate, Guid transactionId, bool forceReconnect = false)
        {

            List<SalesforceQueryEvent> Result = new List<SalesforceQueryEvent>();
            lastModifiedDate = lastModifiedDate ?? "2001-01-01T12:00:00.000Z";

            try
            {
                // Authenticate with Salesforce
                var client = AuthenticateUser(transactionId, forceReconnect);

                //Events
                Result = client.QueryAsync<SalesforceQueryEvent>($" SELECT Id, ActivityDateTime__c, LastModifiedDate, Subject, Description, EventSubtype " +
                                                                 $" FROM Event " +
                                                                 $" WHERE Who.Type = 'Lead' AND Who.Id = '{leadId}' " +
                                                                 $" AND  LastModifiedDate > {lastModifiedDate}" +
                                                                 $" ORDER BY LastModifiedDate ASC NULLS FIRST ").Result?.Records;

                Result = Result ?? new List<SalesforceQueryEvent>();
                LogManager.LogJournalInfo(transactionId, $"Salesforce GetEvents from Salesforce for lead {leadId}", Result);

            }
            catch (Exception ex)
            {
                if (!forceReconnect &&
                    (ex.Message.ToLowerInvariant().Contains("expired") || ex.InnerException.Message.ToLowerInvariant().Contains("expired")))
                {
                    LogManager.LogJournalException(transactionId, new Exception($"Salesforce session expired re-connecting", ex));
                    return GetEvents(leadId, lastModifiedDate, transactionId, true);
                }
                else
                {
                    throw ex;
                }
            }

            return Result;
        }


        public List<SalesforceQueryNote> GetNotes(string leadId, string lastModifiedDate, Guid transactionId, bool forceReconnect = false)
        {

            List<SalesforceQueryNote> Result = new List<SalesforceQueryNote>();
            lastModifiedDate = lastModifiedDate ?? "2001-01-01T12:00:00.000Z";

            try
            {
                // Authenticate with Salesforce
                var client = AuthenticateUser(transactionId, forceReconnect);

                //Linked notes
                var linkedNotes = client.QueryAsync<ContentDocumentLink>($" SELECT ContentDocumentId " +
                                                                         $" FROM ContentDocumentLink " +
                                                                         $" WHERE LinkedEntityId = '{leadId}' " +
                                                                         $" AND ContentDocument.FileType = 'SNOTE'").Result?.Records;

                if (linkedNotes != null && linkedNotes.Count > 0)
                {
                    string notesList = String.Join(",", linkedNotes.Select(x => "'" + x.ContentDocumentId + "'").ToArray());

                    Result = client.QueryAsync<SalesforceQueryNote>($" SELECT Id, Title, Content, LastModifiedDate, CreatedDate  " +
                                                                   $" FROM ContentNote " +
                                                                   $" WHERE Id in ({notesList}) " +
                                                                   $" AND LastModifiedDate > {lastModifiedDate}" +
                                                                   $" ORDER BY LastModifiedDate ASC NULLS FIRST ").Result?.Records;


                }

                LogManager.LogJournalInfo(transactionId, $"Salesforce GetNotes from Salesforce for lead {leadId}", Result);

            }
            catch (Exception ex)
            {
                if (!forceReconnect &&
                    (ex.Message.ToLowerInvariant().Contains("expired") || ex.InnerException.Message.ToLowerInvariant().Contains("expired")))
                {
                    LogManager.LogJournalException(transactionId, new Exception($"Salesforce session expired re-connecting", ex));
                    return GetNotes(leadId, lastModifiedDate, transactionId, true);
                }
                else
                {
                    throw ex;
                }
            }

            return Result;
        }



        private SalesforceLead MapSalesforceLeadFromSalesforceQueryLead(SalesforceQueryLead salesforceQueryLead)
        {
            SalesforceLead Result = new SalesforceLead()
            {
                SalesforceId = salesforceQueryLead.Id,
                CurrentStatus = salesforceQueryLead.Status,
                CurrentSubStatus = salesforceQueryLead.Lead_Sub_Status__c,
                CurrentState = salesforceQueryLead.Lead_State__c,
                LeadOwner = salesforceQueryLead.Assigned_Advisor__r?.Name,
                FirstName = salesforceQueryLead.FirstName,
                LastName = salesforceQueryLead.LastName,
                Email = salesforceQueryLead.Email,
                Phone1 = salesforceQueryLead.Phone,
                Phone2 = salesforceQueryLead.Other_Phone__c,
                Address1 = salesforceQueryLead.Street,
                City = salesforceQueryLead.City,
                StateProvince = salesforceQueryLead.State,
                PostalCode = salesforceQueryLead.PostalCode,
                InstitutionSalesforceId = salesforceQueryLead.Account__c,
                ProgramProductSalesforceId = salesforceQueryLead.Degree__c,
                Notes = salesforceQueryLead.Notes__c,
                HasOptedOutOfEmail = salesforceQueryLead.HasOptedOutOfEmail,
                ClosedReasonCode = salesforceQueryLead.Closed_Reason_Code__c,
                IsTest = salesforceQueryLead.IsTest__c,
                StartTerm = salesforceQueryLead.Start_Term__r?.Name,
                CurrentClientStatus = salesforceQueryLead.Client_Status__c,
                RewarmingIndicator = salesforceQueryLead.RewarmingIndicator__c,
                IsStealthApp = salesforceQueryLead.Stealth_App__c,
                AppDialOutcome = salesforceQueryLead.App_Dial_Outcome__c,
                RewarmingContactAttempts = salesforceQueryLead.Rewarming_Contact_Attempts__c,
                RewarmingDialOutcome = salesforceQueryLead.Rewarming_Dial_Outcome__c,
                HasOptedOutOfSMS = salesforceQueryLead.tdc_tsw__SMS_Opt_out__c,
                UTM_Channel = salesforceQueryLead.Utm_Channel__c,
                InvalidWrongNumberEmailStrategy = salesforceQueryLead.MCC_Invalid_Wrong_Email_Strategy__c,
                EmailCommunicationOnly = salesforceQueryLead.MCC_Email_Communication_Only__c,
                LastDialOutcome = salesforceQueryLead.Last_Dial_Outcome__c,
                LastCallDate = salesforceQueryLead.Last_Dial_Date__c,
                NextCallTime = salesforceQueryLead.Next_Dial_Time__c,
                PreviousCallTime = salesforceQueryLead.Previous_Dial_Time__c,
                NextScheduledSMSDate = salesforceQueryLead.Next_SMS_Date__c,
                PreviousScheduledSMSDate = salesforceQueryLead.Previous_SMS_Date__c,
                PipelineNotes = salesforceQueryLead.Pipeline_Notes__c
            };

            if (salesforceQueryLead.Country == "United States")
            {
                Result.CountryCode = "US";
            }
            else if (salesforceQueryLead.Country == "Canada")
            {
                Result.CountryCode = "CA";
            }
            else 
            {
                Result.CountryCode = salesforceQueryLead.Country;
            }

            int educationLevelId = 0;
            if(int.TryParse(salesforceQueryLead.Education_Level__c, out educationLevelId))
            {
                Result.EducationLevelId = educationLevelId;
            }

            Guid leadGuid = Guid.Empty;
            if(Guid.TryParse(salesforceQueryLead.LeadGUID__c, out leadGuid))
            {
                Result.LeadGUID = leadGuid;
            }

            double ssc = 0;
            if (double.TryParse(salesforceQueryLead.Automated_SMS_Count__c, out ssc))
            {
                Result.ScheduledSMSCount = (int)ssc;
            }

            double saob = 0;
            if (double.TryParse(salesforceQueryLead.MCC_StealthApp_Total_Outbound_Dials__c, out saob))
            {
                Result.StealthAppTotalOutboundDials = (int)saob;
            }

            double tofs = 0;
            if (double.TryParse(salesforceQueryLead.MCC_Total_Outbound_Dials_from_Sub_Status__c, out tofs))
            {
                Result.TotalOutboundDialsFromSubStatus = (int)tofs;
            }

            double lca = 0;
            if (double.TryParse(salesforceQueryLead.Lead_Last_Dial_Age__c, out lca))
            {
                Result.LastCallAge = (int)lca;
            }

            return Result;
        }

    }
}
