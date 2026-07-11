using Azure.Core;
using EDDY.IS.EmsLeadEngine.Core.DataModel;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Entities.AzureFunction;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public class APIRequestConverter
    {
        protected ServiceRules serviceRules = new ServiceRules();

        private Constants.ConversionAPIEvent GetEvent(LeadClientInfo lci, Constants.ConversionAPIEvent defaultValue = Constants.ConversionAPIEvent.Lead)
        {
            Constants.ConversionAPIEvent ev = defaultValue;

            if (lci != null)
            {
                if (lci.StartDate.HasValue || !string.IsNullOrEmpty(lci.ApplicationStartTerm))
                {
                    ev = Constants.ConversionAPIEvent.Started;
                }
                else if (lci.ApplicationDate.HasValue)
                {
                    ev = Constants.ConversionAPIEvent.Applied;
                }
            }          
            return ev;
        }

        private DateTime GetEventDate(LeadClientInfo lci)
        {
            DateTime eventDate = DateTime.Now;

            if (lci != null)
            {
                if (lci.StartDate.HasValue || !string.IsNullOrEmpty(lci.ApplicationStartTerm))
                {
                    DateTime parsedStartTerm;
                    bool successfullyParsedDateTime = DateTime.TryParse(lci.ApplicationStartTerm, out parsedStartTerm);

                    if (lci.StartDate.HasValue)
                        eventDate = lci.StartDate.GetValueOrDefault();
                    else if (successfullyParsedDateTime)
                        eventDate = parsedStartTerm;
                }
                else if (lci.ApplicationDate.HasValue)
                {
                    eventDate = lci.ApplicationDate.GetValueOrDefault();
                }
            }

            return eventDate;
        }

        private Constants.ConversionAPIEvent GetCustomEvent(Lead EmsLead, LeadClientInfo lci, List<CustomEventInstitutionMapping> customEventRules, Constants.ConversionAPIEvent defaultValue = Constants.ConversionAPIEvent.Lead)
        {
            Constants.ConversionAPIEvent ev = defaultValue;
            if (EmsLead != null)
            {
                try
                {
                    bool eventResultFound = false;
                    foreach (CustomEventInstitutionMapping rule in customEventRules.OrderByDescending(r => r.ConversionAPIEventId))
                    {
                        switch (rule.FieldType)
                        {
                            case "LeadStatusId":
                                if (EmsLead.LeadStatusId.ToString() == rule.FieldValue)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                    
                                break;
                            case "LeadSubStatusId":
                                if (EmsLead.LeadSubStatusId.ToString() == rule.FieldValue)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                break;
                            case "ClientStatus":
                                if (lci.Status == rule.FieldValue)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                break;
                            case "ClientStartDate":
                                if (lci.StartDate != null)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                break;
                            case "ClientApplicationStartTerm":
                                if (lci.ApplicationStartTerm != null)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                break;
                            case "ClientApplicationDate":
                                if (lci.ApplicationDate != null)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                break;
                            case "ClientEnrollDate":
                                if (lci.EnrollDate != null)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                break;
                            case "ClientApplicationStartDate":
                                if (lci.ApplicationStartDate != null)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                break;
                            case "ClientAdmitDate":
                                if (lci.AdmitDate != null)
                                {
                                    ev = (Constants.ConversionAPIEvent)rule.ConversionAPIEventId;
                                    eventResultFound = true;
                                }
                                break;
                            default:
                                break;
                        }
                        if (eventResultFound)
                            break;
                    }

                }

                catch (Exception ex)
                {

                }
            }
            return ev;
        }


        #region Helpers
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
        #endregion

        #region Senders
        private void SendFBConversionAPIRequest(string methodName, FBConversionAPIRequest request)
        {
            request.DatabaseServer = DataService.GetMarketingDatabaseServerName();
            AzureFunctionServices.SendFBConversionAPIRequest(request);

            string jsonMessage = JsonConvert.SerializeObject(request);
            DataService.LogFBConversionAttemptInfo(request.EMSLeadId, (int)request.Event, jsonMessage, request.ISLeadId);
        }

        private void SendFBAudienceRequest(string methodName, FBAudienceRequest request)
        {
            request.DatabaseServer = DataService.GetMarketingDatabaseServerName();
            AzureFunctionServices.SendFBAudienceRequest(request);
            
            string jsonMessage = JsonConvert.SerializeObject(request);
            DataService.LogFBAudienceAttemptInfo(request.ISLeadId, (int)request.Event, jsonMessage, request.EMSLeadId);
        }

        private void SendGoogleConversionAPIRequest(string methodName, GoogleConversionAPIRequest request)
        {
            request.DatabaseServer = DataService.GetMarketingDatabaseServerName();
            AzureFunctionServices.SendGoogleConversionAPIRequest(request);
            
            string jsonMessage = JsonConvert.SerializeObject(request);
            DataService.LogGoogleConversionAttemptInfo(request.ISLeadId, request.EMSLeadId, (int)request.Event, jsonMessage);
        }

        private void SendGoogleAudienceRequest(string methodName, GoogleAudienceAPIRequest request)
        {
            request.DatabaseServer = DataService.GetMarketingDatabaseServerName();
            AzureFunctionServices.SendGoogleAudienceRequest(request);

            string jsonMessage = JsonConvert.SerializeObject(request);
            DataService.LogGoogleAudienceAttemptInfo(request.ISLeadId, request.EMSLeadId, (int)request.Event, jsonMessage);
        }
        #endregion

        public void Process(Lead EMSLead,string methodName, LeadClientInfo lci, List<CustomEventInstitutionMapping> customEventRules = null)
        {

            //DEFAULT DATA
            string trackid = null;
            VW_ISLead ISLead = null;
            VW_ProgramMapping programMapping = null;
            Constants.ConversionAPIEvent capiEvent = Constants.ConversionAPIEvent.Lead;
            DateTime capiEventDate = DateTime.Now;

            //Event Data
            if(customEventRules == null)
                customEventRules = serviceRules.GetCustomEventInstitutionMappings(EMSLead.InstitutionId);

            capiEvent = customEventRules.Any() ? GetCustomEvent(EMSLead, lci, customEventRules) : GetEvent(lci);

            capiEventDate = GetEventDate(lci);

            //GET DATA USED FOR CAPI/AUDIENCES
            if (EMSLead.ISLeadId != null)
                ISLead = DataService.GetISLead(EMSLead.ISLeadId.GetValueOrDefault());
            if (EMSLead.MediaPlanItemId.GetValueOrDefault() > 0) // TrackId only needed for CAPI
                trackid = DataService.GetTrackIdForMediaPlanItem(EMSLead.MediaPlanItemId.GetValueOrDefault());

            SendData sendData = new SendData
            {
                FacebookCAPI = ConfigurationManager.AppSettings["SendToFBConversion"] == "true" && trackid != null && DataService.CheckCampaignSendsEventToFBAPI(trackid, (int)capiEvent),
                FacebookAudience = ConfigurationManager.AppSettings["SendToFBAudience"] == "true" && DataService.CheckInstitutionSendsToFBAudience(EMSLead.InstitutionId),
                GoogleCAPI = ConfigurationManager.AppSettings["SendToGoogleConversion"] == "true" && trackid != null && DataService.CheckCampaignSendsToGoogleAPI(trackid, (int)capiEvent),
                GoogleAudience = ConfigurationManager.AppSettings["SendToGoogleAudience"] == "true" && DataService.CheckInstitutionSendsToGoogleAudience(EMSLead.InstitutionId)
            };

            //We want to skip all of this logic if we dont need to send to FB or Google
            if (!(sendData.FacebookAudience || sendData.FacebookCAPI || sendData.GoogleAudience || sendData.GoogleCAPI))
                return;
            
            if(EMSLead.ISProgramProductId != null && (sendData.FacebookAudience || sendData.GoogleAudience)) // Program Mapping only needed for Audiences
                programMapping = DataService.GetEMSProgramMapping(EMSLead.ISProgramProductId);

            //FB CAPI
            if (sendData.FacebookCAPI)
            {
                FBConversionAPIRequest req = APIRequestFactory.GetFBConversionAPIRequest(EMSLead, ISLead, capiEvent, trackid, capiEventDate);              
                SendFBConversionAPIRequest(methodName, req);
            }

            //FB AUDIENCE
            if (sendData.FacebookAudience)
            {
                FBAudienceRequest areq = APIRequestFactory.GetFBAudienceRequest(EMSLead, ISLead, capiEvent, capiEventDate, programMapping);            
                SendFBAudienceRequest(methodName, areq);
            }

            //GOOGLE CAPI
            if (sendData.GoogleCAPI)
            {
                GoogleConversionAPIRequest req = APIRequestFactory.GetGoogleConversionAPIRequest(EMSLead, ISLead, capiEvent, trackid, capiEventDate);             
                SendGoogleConversionAPIRequest(methodName, req);           
            }

            //GOOGLE AUDIENCE
            if (sendData.GoogleAudience)
            {
                GoogleAudienceAPIRequest req = APIRequestFactory.GetGoogleAudienceRequest(EMSLead, ISLead, capiEvent, capiEventDate, programMapping);            
                SendGoogleAudienceRequest(methodName, req);
            }
        }

        class SendData {
            public bool FacebookCAPI { get; set; }
            public bool FacebookAudience { get; set; }
            public bool GoogleCAPI { get; set; }
            public bool GoogleAudience { get; set; }
        }
    }
}
