using EDDY.IS.Base;
using EDDY.IS.Core.Logging;
using EDDY.IS.EmsLeadEngine.Core.DataModel;
using EDDY.IS.EmsLeadEngine.Core.Extensions;
using EDDY.IS.EmsLeadEngine.Core.Properties;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Entities.AzureFunction;
using EDDY.IS.EmsLeadEngine.Entities.Common;
using Microsoft.ServiceBus.Messaging;
using EDDY.IS.EmsLeadEngine.Entities.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public class EmsLeadEngineBase
    {
        protected ServiceRules serviceRules = new ServiceRules();
        protected EMSPostUpServiceClient.EMSPostUpServiceClient postUpServiceClient = new EMSPostUpServiceClient.EMSPostUpServiceClient();
        private EMSDataService _DataService;
        public EMSDataService DataService
        {
            get
            {
                if (_DataService == null)
                {
                    _DataService = new EMSDataService();
                }
                return _DataService;
            }
        }

        private AzureFunctionHelper _AzureFunctionServices;

        private AzureFunctionHelper AzureFunctionServices
        {
            get
            {
                if (_AzureFunctionServices == null)
                {
                    _AzureFunctionServices = new AzureFunctionHelper();
                }
                return _AzureFunctionServices;
            }
        }

        private EntityMappings _Mappings;

        public EntityMappings Mappings
        {
            get
            {
                if (_Mappings == null)
                {
                    _Mappings = new EntityMappings();
                }
                return _Mappings;
            }
        }

        private EventGridHelper _EventGridServices;

        private EventGridHelper EventGridServices
        {
            get
            {
                if (_EventGridServices == null)
                {
                    _EventGridServices = new EventGridHelper();
                }
                return _EventGridServices;
            }
        }

        public void SendAzureLeadInsertRequest(string methodName, InsertLeadRequest request)
        {
            request.DatabaseServer = DataService.GetDatabaseServerName();
            request.UseSalesforceSandbox = Settings.Default.UseSalesforceSandbox;
            AzureFunctionServices.SendInsertLeadRequest(request);
            LogManager.LogJournalInfo(request.TransactionId, request.SubTransactionId, $"{methodName} EMSLeadId={request.EMSLeadId} sent to Azure Insert queue.");
        }

        public void SendAzureLeadUpdateRequest(string methodName, UpdateLeadRequest request)
        {
            request.DatabaseServer = DataService.GetDatabaseServerName();
            request.UseSalesforceSandbox = Settings.Default.UseSalesforceSandbox;
            AzureFunctionServices.SendUpdateLeadRequest(request);
            LogManager.LogJournalInfo(request.TransactionId, request.SubTransactionId, $"{methodName} EMSLeadId={request.EMSLeadId} sent to Azure Update queue.");
        }

        public void SendFBConversionAPIRequest(string methodName, FBConversionAPIRequest request)
        {
            request.DatabaseServer = DataService.GetMarketingDatabaseServerName();
            AzureFunctionServices.SendFBConversionAPIRequest(request);
            string jsonMessage = JsonConvert.SerializeObject(request);
            DataService.LogFBConversionAttemptInfo(request.EMSLeadId, (int)request.Event, jsonMessage, request.ISLeadId);
        }

        public void SendFBAudienceRequest(string methodName, FBAudienceRequest request)
        {
            request.DatabaseServer = DataService.GetMarketingDatabaseServerName();
            AzureFunctionServices.SendFBAudienceRequest(request);
            string jsonMessage = JsonConvert.SerializeObject(request);
            DataService.LogFBAudienceAttemptInfo(request.ISLeadId, (int)request.Event, jsonMessage, request.EMSLeadId);
        }

        public void SendGoogleConversionAPIRequest(string methodName, GoogleConversionAPIRequest request)
        {
            request.DatabaseServer = DataService.GetMarketingDatabaseServerName();
            AzureFunctionServices.SendGoogleConversionAPIRequest(request);
            string jsonMessage = JsonConvert.SerializeObject(request);
            DataService.LogGoogleConversionAttemptInfo(request.ISLeadId, request.EMSLeadId, (int)request.Event, jsonMessage);
        }

        public void SendPostUpRequest(long leadId, string methodName, Guid transactionId)
        {
            postUpServiceClient.SendPostUpRequest(leadId);
            LogManager.LogJournalInfo(transactionId, $"{methodName} EMSLeadId={leadId} sent to Post Up Service Client");
        }

        public void SendEventGridRequest(long leadId, string methodName, List<EventGridEvent> request, Guid transactionId)
        {
            EventGridServices.SendEventGridRequest(request);
            LogManager.LogJournalInfo(transactionId, $"{methodName} EMSLeadId={leadId} sent to Event Grid");
        }

        public LeadHistory CreateNewLeadHistory(long leadId, Constants.LeadSourceType type, Dictionary<string, string> additionalQuestions)
        {
            LeadHistory Result = new LeadHistory()
            {
                LeadId = leadId,
                LeadSourceTypeId = (short)type,
                Timestamp = DateTime.Now,
                JsonData = JsonConvert.SerializeObject(additionalQuestions, Formatting.Indented)
            };

            return Result;
        }
        public LeadMarketingAttribution CreateNewLeadMarketingAttribution(long leadId, Dictionary<string, string> additionalQuestions)
        {
            LeadMarketingAttribution Result = new LeadMarketingAttribution();
            Result.LeadId = leadId;

            Result.Keyword = GetFieldValueFromAdditionalQuestions(additionalQuestions, "Keyword", Result.Keyword);
            Result.Keyword = GetFieldValueFromAdditionalQuestions(additionalQuestions, "SearchKeyword", Result.Keyword);
            Result.FormName = GetFieldValueFromAdditionalQuestions(additionalQuestions, "FormName", Result.FormName);
            Result.utm_medium = GetFieldValueFromAdditionalQuestions(additionalQuestions, "utm_medium", Result.utm_medium);
            Result.utm_source = GetFieldValueFromAdditionalQuestions(additionalQuestions, "utm_source", Result.utm_source);
            Result.utm_term = GetFieldValueFromAdditionalQuestions(additionalQuestions, "utm_term", Result.utm_term);
            Result.utm_content = GetFieldValueFromAdditionalQuestions(additionalQuestions, "utm_content", Result.utm_content);
            Result.utm_campaign = GetFieldValueFromAdditionalQuestions(additionalQuestions, "utm_campaign", Result.utm_campaign);
            Result.AdGroup = GetFieldValueFromAdditionalQuestions(additionalQuestions, "AdGroup", Result.AdGroup);
            Result.SourceCode = GetFieldValueFromAdditionalQuestions(additionalQuestions, "SourceCode", Result.SourceCode);
            Result.CaptureURL = GetFieldValueFromAdditionalQuestions(additionalQuestions, "FormLeadUrl", Result.CaptureURL);

             string decodedUrl = HttpUtility.UrlDecode(Result.CaptureURL);

            if (!String.IsNullOrEmpty(decodedUrl))
            {
                Result.utm_campaign = GetUtmValueFromUrl(decodedUrl, Result.utm_campaign, "utm_campaign__c", "utm_campaign");
                Result.utm_medium = GetUtmValueFromUrl(decodedUrl, Result.utm_medium, "utm_medium__c", "utm_medium");
                Result.utm_source = GetUtmValueFromUrl(decodedUrl, Result.utm_source, "utm_source__c", "utm_source");
                Result.utm_term = GetUtmValueFromUrl(decodedUrl, Result.utm_term, "utm_term__c", "utm_term");
                Result.utm_content = GetUtmValueFromUrl(decodedUrl, Result.utm_content, "utm_content__c", "utm_content");  
            }

            return Result;
        }


        private string GetUtmValueFromUrl(string decodedUrl, string currentValue, string nameVariation1, string nameVariation2)
        {
            if (String.IsNullOrEmpty(currentValue))
            {
                string queryString = decodedUrl;

                if (Uri.TryCreate(decodedUrl, UriKind.Absolute, out var uri))
                {
                    queryString = uri.Query;
                }

                var queryParams = HttpUtility.ParseQueryString(queryString);

                string utmValue1 = queryParams.Get(nameVariation1);
                string utmValue2 = queryParams.Get(nameVariation2);

                if (!String.IsNullOrEmpty(utmValue1))
                {
                    return utmValue1;
                }
                else if (!String.IsNullOrEmpty(utmValue2))
                {
                    return utmValue2;
                }
            }
            return currentValue;
        }
        private string GetFieldValueFromAdditionalQuestions(Dictionary<string, string> additionalQuestions, string key, string defaultValue)
        {
            string result = additionalQuestions.FirstOrDefault(x => String.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).Value;
            return string.IsNullOrWhiteSpace(result) ? defaultValue : result;
        }

        protected LeadClientInfo UpsertLeadClientInfo(ClientInfoLead lead, long emsLeadId, string methodName, Guid transactionId)
        {
            var existingLeadClientInfo = DataService.GetLeadClientInfo(emsLeadId);
            LeadClientInfo lci = null;
            if (existingLeadClientInfo != null)
            {
                lci = Mappings.MergeLeadClientInfoFromUpdate(existingLeadClientInfo, lead);
                UpdateLeadClientInfo(lci, emsLeadId, methodName, transactionId);
            }
            else
            {
                lci = CreateLeadClientInfo(lead, emsLeadId, methodName, transactionId);
            }
            return lci;
        }

        protected LeadClientInfo UpsertLeadClientInfo(ClientInfoLeadBase lead, long emsLeadId, string methodName, Guid transactionId)
        {
            LeadClientInfo lci = null;
            var existingLeadClientInfo = DataService.GetLeadClientInfo(emsLeadId);

            if (existingLeadClientInfo != null)
            {
                lci = Mappings.MergeLeadClientInfoBaseFromUpdate(existingLeadClientInfo, lead);
                UpdateLeadClientInfo(lci, emsLeadId, methodName, transactionId);
            }
            else
            {
                lci = CreateLeadClientInfo(lead, emsLeadId, methodName, transactionId);
            }
            return lci;
        }

        protected LeadSalesforceInfo UpsertLeadSalesforceInfo(SalesforceInfoLead lead, long emsLeadId, string methodName, Guid transactionId)
        {
            LeadSalesforceInfo lsi = null;
            var existingLeadSalesforceInfo = DataService.GetLeadSalesforceInfo(emsLeadId);

            if (existingLeadSalesforceInfo != null)
            {
                lsi = Mappings.MergeLeadSalesforceInfoFromUpdate(existingLeadSalesforceInfo, lead);
                UpdateLeadSalesforceInfo(lsi, emsLeadId, methodName, transactionId);
            }
            else
            {
                lsi = CreateLeadSalesforceInfo(lead, emsLeadId, methodName, transactionId);
            }
            return lsi;
        }

        protected LeadSalesforceInfo CreateLeadSalesforceInfo(SalesforceInfoLead lead, long emsLeadId, string methodName, Guid transactionId)
        {
            LeadSalesforceInfo lsi = null;
            if (LeadSalesforceInfoIsNotEmpty(lead))
            {
                lsi = Mappings.MapLeadSalesforceInfoFromSalesforceInfoLead(lead, emsLeadId);
                CreateLeadSalesforceInfo(lsi, methodName, transactionId);
            }
            return lsi;
        }

        protected long CreateLeadSalesforceInfo(LeadSalesforceInfo leadSalesforceInfo, string methodName, Guid transactionId)
        {
            long leadSalesforceInfoId = 0;
            leadSalesforceInfoId = DataService.CreateLeadSalesforceInfo(leadSalesforceInfo);
            LogManager.LogJournalInfo(transactionId, $"{methodName} LeadSalesforceInfo inserted, LeadSalesforceInfoId={leadSalesforceInfoId}");
            return leadSalesforceInfoId;
        }

        protected LeadClientInfo CreateLeadClientInfo(ClientInfoLead lead, long emsLeadId, string methodName, Guid transactionId)
        {
            LeadClientInfo lci = null;
            if (LeadClientInfoIsNotEmpty(lead))
            {
                lci = Mappings.MapLeadClientInfoFromClientInfoLead(lead, emsLeadId);
                CreateLeadClientInfo(lci, methodName, transactionId);
            }
            return lci;
        }

        protected LeadClientInfo CreateLeadClientInfo(ClientInfoLeadBase lead, long emsLeadId, string methodName, Guid transactionId)
        {
            LeadClientInfo lci = null;
            if (LeadClientInfoIsNotEmpty(lead))
            {
                lci = Mappings.MapLeadClientInfoFromClientInfoLeadBase(lead, emsLeadId);
                CreateLeadClientInfo(lci, methodName, transactionId);
            }
            return lci;
        }

        protected void CreateLeadMarketingAttribution(ExchangeLead lead, long emsLeadId, string methodName, Guid transactionId)
        {
            if (LeadMarketingAttributionIsNotEmpty(lead))
            {
                CreateLeadMarketingAttribution(Mappings.MapLeadLeadMarketingAttributionFromExchangeLead(lead, emsLeadId), methodName, transactionId);
            }
        }

        protected long CreateLeadClientInfo(LeadClientInfo leadClientInfo, string methodName, Guid transactionId)
        {
            long leadClientInfoId = 0;
            leadClientInfoId = DataService.CreateLeadClientInfo(leadClientInfo);
            LogManager.LogJournalInfo(transactionId, $"{methodName} LeadClientInfo inserted, LeadClientInfoId={leadClientInfoId}");
            return leadClientInfoId;
        }

        protected long CreateLeadMarketingAttribution(LeadMarketingAttribution leadMarketingAttribution, string methodName, Guid transactionId)
        {
            long leadMarketingAttributionId = 0;
            leadMarketingAttributionId = DataService.CreateLeadMarketingAttribution(leadMarketingAttribution);
            LogManager.LogJournalInfo(transactionId, $"{methodName} LeadMarketingAttribution inserted, LeadMarketingAttributionId={leadMarketingAttributionId}");
            return leadMarketingAttributionId;
        }

        private void UpdateLeadClientInfo(LeadClientInfo leadClientInfo, long emsLeadId, string methodName, Guid transactionId)
        {
            var updatedSuccessfully = DataService.UpdateLeadClientInfo(leadClientInfo);
            var message = $"{methodName} LeadClientInfo found by EMSLeadId {emsLeadId}, {{0}}, LeadClientInfoId={leadClientInfo.LeadClientInfoId}";
            var innerMessage = updatedSuccessfully ? "and was updated" : "but failed to update";
            LogManager.LogJournalInfo(transactionId, string.Format(message, innerMessage));
        }

        private void UpdateLeadSalesforceInfo(LeadSalesforceInfo leadSalesforceInfo, long emsLeadId, string methodName, Guid transactionId)
        {
            var updatedSuccessfully = DataService.UpdateLeadSalesforceInfo(leadSalesforceInfo);
            var message = $"{methodName} LeadSalesforceInfo found by EMSLeadId {emsLeadId}, {{0}}, LeadSalesforceInfo={leadSalesforceInfo.LeadSalesforceInfoId}";
            var innerMessage = updatedSuccessfully ? "and was updated" : "but failed to update";
            LogManager.LogJournalInfo(transactionId, string.Format(message, innerMessage));
        }

        private bool LeadClientInfoIsNotEmpty(ClientInfoLead lead)
        {
            return !string.IsNullOrWhiteSpace(lead.ClientStatus)
                || !string.IsNullOrWhiteSpace(lead.ClientNotes)
                || !string.IsNullOrWhiteSpace(lead.ClientInitialStartTerm)
                || !string.IsNullOrWhiteSpace(lead.ClientApplicationStartTerm)
                || !string.IsNullOrWhiteSpace(lead.ClientApplicationDegreeName)
                || !string.IsNullOrWhiteSpace(lead.ClientRegistered)
                || lead.ClientStatusUpdatedDate.HasValue
                || lead.ClientApplicationDate.HasValue
                || !string.IsNullOrWhiteSpace(lead.CustomFields)
                || lead.ClientInterviewDate.HasValue
                || lead.ClientStartDate.HasValue
                || lead.ClientStartReceivedDate.HasValue
                || lead.ClientContactDate.HasValue
                || lead.ClientEnrollDate.HasValue
                || lead.ClientApplicationStartDate.HasValue
                || lead.ClientAdmitDate.HasValue
                || lead.ClientAppointmentDate.HasValue
                || lead.ClientQualifiedDate.HasValue
                || lead.ClientGraduateDate.HasValue
                || lead.ClientFirstTermPersistDate.HasValue
                || lead.ClientFAFSAReceivedDate.HasValue
                || lead.ClientApplicationSubmittedDate.HasValue
                || lead.ClientDepositDate.HasValue
                || lead.ClientApplicationDeniedDate.HasValue
                || !string.IsNullOrWhiteSpace(lead.CompletedApplicationChecklistItems)
                || !string.IsNullOrWhiteSpace(lead.PendingApplicationChecklistItems)
                || !string.IsNullOrWhiteSpace(lead.PipelineNotes);
        }

        private bool LeadSalesforceInfoIsNotEmpty(SalesforceInfoLead lead)
        {
            return lead.InvalidWrongNumberEmailStrategy.HasValue
                || lead.LastCallAge.HasValue
                || lead.LastCallDate.HasValue
                || !string.IsNullOrWhiteSpace(lead.LastDialOutcome)
                || lead.NextCallTime.HasValue
                || lead.NextScheduledSMSDate.HasValue
                || lead.PreviousCallTime.HasValue
                || lead.PreviousScheduledSMSDate.HasValue
                || lead.StealthAppTotalOutboundDials.HasValue
                || lead.TotalOutboundDialsFromSubStatus.HasValue
                || lead.ScheduledSMSCount.HasValue;
        }

        private bool LeadClientInfoIsNotEmpty(ClientInfoLeadBase lead)
        {
            return !string.IsNullOrWhiteSpace(lead.ClientStatus)
                || !string.IsNullOrWhiteSpace(lead.ClientNotes)
                || !string.IsNullOrWhiteSpace(lead.ClientInitialStartTerm)
                || !string.IsNullOrWhiteSpace(lead.PipelineNotes)
                || !string.IsNullOrWhiteSpace(lead.PendingApplicationChecklistItems)
                || !string.IsNullOrWhiteSpace(lead.CompletedApplicationChecklistItems);
        }

        private bool LeadMarketingAttributionIsNotEmpty(ExchangeLead lead)
        {
            return !string.IsNullOrWhiteSpace(lead.lma_utm_term)
                || !string.IsNullOrWhiteSpace(lead.lma_utm_source)
                || !string.IsNullOrWhiteSpace(lead.lma_utm_medium)
                || !string.IsNullOrWhiteSpace(lead.lma_utm_campaign)
                || !string.IsNullOrWhiteSpace(lead.lma_keyword)
                || !string.IsNullOrWhiteSpace(lead.lma_captureurl);
        }

    }
}
