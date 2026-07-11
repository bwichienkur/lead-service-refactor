using EDDY.IS.EmsLeadEngine.Core.DataModel;
using EDDY.IS.EmsLeadEngine.Core.Extensions;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Entities.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public class EntityMappings
    {
        protected ServiceRules serviceRules = new ServiceRules();
        public Lead MapEMSLeadFromExchange(ExchangeLead source)
        {
            int defaultStatusId = Constants.STATUS_NEW_ID;
            int? defaultSubStatusId = null;

            if (serviceRules.CheckIfMigratedInstitutionMCC(source.EMSInstitutionId))
            {
                defaultStatusId = Constants.MCC_STATUS_NEW_ID;
                defaultSubStatusId = Constants.MCC_SUBSTATUS_NEW_ID;
            }


            Lead result = new Lead()
            {
                Address1 = ValidationExtensions.SetValueToNullIfEmpty(source.Address1),
                Address2 = ValidationExtensions.SetValueToNullIfEmpty(source.Address2),
                City = ValidationExtensions.SetValueToNullIfEmpty(source.City),
                CountryCode = ValidationExtensions.SetValueToNullIfEmpty(source.CountryCode),
                PostalCode = ValidationExtensions.SetValueToNullIfEmpty(source.PostalCode),
                LeadStatusId = source.LeadStatusId.HasValue ? source.LeadStatusId.Value : defaultStatusId,
                LeadSubStatusId = source.LeadSubStatusId.HasValue ? source.LeadSubStatusId.Value : defaultSubStatusId,
                LeadStateId = source.LeadStateId.HasValue ? source.LeadStateId.Value : Constants.STATE_OPEN_ID,
                CreatedDate = source.CreatedDate.HasValue ? source.CreatedDate.Value : DateTime.Now,
                UpdatedDate = DateTime.Now,
                EducationLevelId = source.EducationLevelId,
                Email = ValidationExtensions.SetValueToNullIfEmpty(source.Email),
                InstitutionId = source.EMSInstitutionId,
                ExternalId = ValidationExtensions.SetValueToNullIfEmpty(source.ExternalId),
                FirstName = ValidationExtensions.SetValueToNullIfEmpty(source.FirstName),
                LeadGUID = source.LeadGUID.HasValue ? source.LeadGUID.Value : Guid.NewGuid(),
                LegacyGPLeadId = ValidationExtensions.SetValueToNullIfEmpty(source.LegacyGPLeadId),
                LastName = string.IsNullOrWhiteSpace(source.LastName) ? "-" : source.LastName,
                LeadSourceTypeId = (int)Constants.LeadSourceType.DataExchange,
                Phone1 = ValidationExtensions.SetValueToNullIfEmpty(DataUtils.ParsePhone(source.Phone1)),
                Phone2 = ValidationExtensions.SetValueToNullIfEmpty(DataUtils.ParsePhone(source.Phone2)),
                ISProgramProductId = source.ProgramProductId.HasValue ? source.ProgramProductId.Value : 0,
                StateProvince = ValidationExtensions.SetValueToNullIfEmpty(source.StateProvince),
                utm_vendor = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Vendor),
                utm_channel = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Channel),
                utm_campaign = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Campaign),
                IsTest = !source.IsTest ? ValidationExtensions.IsTestLead(source.Email) : source.IsTest,
                HasOptedOutOfEmail = source.HasOptedOutOfEmail,
                MediaPlanItemId = source.MediaPlanItemId,
                ClosedReasonCode = !String.IsNullOrWhiteSpace(source.ClosedReasonCode) ? source.ClosedReasonCode : null
            };

            return result;
        }

        public LeadSalesforceInfo MapLeadSalesforceInfoFromSalesforceInfoLead(SalesforceInfoLead source, long leadId)
        {
            var leadSalesforceInfo = new LeadSalesforceInfo();
            leadSalesforceInfo.EmailCommunicationOnly = source.EmailCommunicationOnly;
            leadSalesforceInfo.InvalidWrongNumberEmailStrategy = source.InvalidWrongNumberEmailStrategy;
            leadSalesforceInfo.LastCallAge = source.LastCallAge;
            leadSalesforceInfo.LastCallDate = source.LastCallDate;
            leadSalesforceInfo.LastDialOutcome = source.LastDialOutcome;
            leadSalesforceInfo.LeadId = leadId;
            leadSalesforceInfo.NextCallTime = source.NextCallTime;
            leadSalesforceInfo.NextScheduledSMSDate = source.NextScheduledSMSDate;
            leadSalesforceInfo.PreviousCallTime = source.PreviousCallTime;
            leadSalesforceInfo.ScheduledSMSCount = source.ScheduledSMSCount;
            leadSalesforceInfo.StealthAppTotalOutboundDials = source.StealthAppTotalOutboundDials;
            leadSalesforceInfo.TotalOutboundDialsFromSubStatus = source.TotalOutboundDialsFromSubStatus;

            return leadSalesforceInfo;
        }

        public LeadClientInfo MapLeadClientInfoFromClientInfoLead(ClientInfoLead source, long leadId)
        {
            var leadClientInfo = MapLeadClientInfoFromClientInfoLeadBase(source, leadId);
            leadClientInfo.ApplicationDegreeName = ValidationExtensions.SetValueToNullIfEmpty(source.ClientApplicationDegreeName);
            leadClientInfo.ApplicationDate = source.ClientApplicationDate;
            leadClientInfo.StatusUpdatedDate = source.ClientStatusUpdatedDate;
            leadClientInfo.InterviewDate = source.ClientInterviewDate;
            leadClientInfo.StartDate = source.ClientStartDate;
            leadClientInfo.CustomFields = source.CustomFields;
            leadClientInfo.StartReceivedDate = source.ClientStartReceivedDate;
            leadClientInfo.ContactDate = source.ClientContactDate;
            leadClientInfo.EnrollDate = source.ClientEnrollDate;
            leadClientInfo.ApplicationStartDate = source.ClientApplicationStartDate;
            leadClientInfo.AdmitDate = source.ClientAdmitDate;
            leadClientInfo.AppointmentDate = source.ClientAppointmentDate;
            leadClientInfo.QualifiedDate = source.ClientQualifiedDate;
            leadClientInfo.GraduateDate = source.ClientGraduateDate;
            leadClientInfo.FirstTermPersistDate = source.ClientFirstTermPersistDate;
            leadClientInfo.FAFSAReceivedDate = source.ClientFAFSAReceivedDate;
            leadClientInfo.Registered = source.ClientRegistered;
            leadClientInfo.ApplicationSubmittedDate = source.ClientApplicationSubmittedDate;
            leadClientInfo.DepositDate = source.ClientDepositDate;
            leadClientInfo.ApplicationDeniedDate = source.ClientApplicationDeniedDate;
            leadClientInfo.CompletedApplicationChecklistItems = source.CompletedApplicationChecklistItems;
            leadClientInfo.PendingApplicationChecklistItems = source.PendingApplicationChecklistItems;
            leadClientInfo.PipelineNotes = source.PipelineNotes;

            return leadClientInfo;
        }

        public LeadClientInfo MapLeadClientInfoFromClientInfoLeadBase(ClientInfoLeadBase source, long leadId)
        {
            return new LeadClientInfo()
            {
                LeadId = leadId,
                Status = ValidationExtensions.SetValueToNullIfEmpty(source.ClientStatus),
                InitialStartTerm = ValidationExtensions.SetValueToNullIfEmpty(source.ClientInitialStartTerm),
                ApplicationStartTerm = ValidationExtensions.SetValueToNullIfEmpty(source.ClientApplicationStartTerm),
                Notes = ValidationExtensions.SetValueToNullIfEmpty(source.ClientNotes),
                PipelineNotes = ValidationExtensions.SetValueToNullIfEmpty(source.PipelineNotes)
            };
        }

        public LeadMarketingAttribution MapLeadLeadMarketingAttributionFromExchangeLead(ExchangeLead source, long leadId)
        {
            return new LeadMarketingAttribution()
            {
                LeadId = leadId,
                utm_campaign = ValidationExtensions.SetValueToNullIfEmpty(source.lma_utm_campaign),
                utm_source = ValidationExtensions.SetValueToNullIfEmpty(source.lma_utm_source),
                utm_term = ValidationExtensions.SetValueToNullIfEmpty(source.lma_utm_term),
                utm_medium = ValidationExtensions.SetValueToNullIfEmpty(source.lma_utm_medium),
                utm_content = ValidationExtensions.SetValueToNullIfEmpty(source.lma_utm_content),
                Keyword = ValidationExtensions.SetValueToNullIfEmpty(source.lma_keyword),
                CaptureURL = ValidationExtensions.SetValueToNullIfEmpty(source.lma_captureurl)
            };
        }

        /// <summary>
        /// Maps request allowed properties to be changed for update purposes, alters original object
        /// </summary>
        /// <param name="original"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Lead MergeEMSLeadFromExchangeUpdate(Lead original, ExchangeLead request)
        {
            original.UpdatedDate = DateTime.Now;

            //protected fields, should only update from data exchange if the existing lead has null or empty value
            original.Address1 = ValidationExtensions.Merge(original.Address1, request.Address1);
            original.Address2 = ValidationExtensions.Merge(original.Address2, request.Address2);
            original.City = ValidationExtensions.Merge(original.City, request.City);
            original.CountryCode = ValidationExtensions.Merge(original.CountryCode, request.CountryCode);
            original.PostalCode = ValidationExtensions.Merge(original.PostalCode, request.PostalCode);
            original.Email = ValidationExtensions.Merge(original.Email, request.Email);
            original.FirstName = ValidationExtensions.Merge(original.FirstName, request.FirstName);
            original.LastName = ValidationExtensions.Merge(original.LastName, request.LastName);
            original.Phone1 = ValidationExtensions.Merge(original.Phone1, DataUtils.ParsePhone(request.Phone1));
            original.Phone2 = ValidationExtensions.Merge(original.Phone2, DataUtils.ParsePhone(request.Phone2));
            original.StateProvince = ValidationExtensions.Merge(original.StateProvince, request.StateProvince);
            original.utm_campaign = ValidationExtensions.Merge(original.utm_campaign, request.UTM_Campaign);
            original.utm_vendor = ValidationExtensions.Merge(original.utm_vendor, request.UTM_Vendor);

            //always replace these fields values if data exchange requests it
            original.ClosedReasonCode = !String.IsNullOrWhiteSpace(request.ClosedReasonCode) ? request.ClosedReasonCode : original.ClosedReasonCode;
            original.LeadStatusId = request.LeadStatusId.HasValue ? request.LeadStatusId.Value : original.LeadStatusId;
            original.LeadSubStatusId = request.LeadSubStatusId.HasValue ? request.LeadSubStatusId.Value : original.LeadSubStatusId;
            original.LeadStateId = request.LeadStateId.HasValue ? request.LeadStateId.Value : original.LeadStateId;
            original.EducationLevelId = request.EducationLevelId.HasValue ? request.EducationLevelId.Value : original.EducationLevelId;
            original.ISProgramProductId = request.ProgramProductId.HasValue ? request.ProgramProductId.Value : original.ISProgramProductId;
            original.ExternalId = !String.IsNullOrWhiteSpace(request.ExternalId) ? request.ExternalId : original.ExternalId;

            //shouldn't even need to update these values from data exchange
            //original.InstitutionId = request.EMSInstitutionId;
            //original.IsTest = !request.IsTest ? ValidationExtensions.IsTestLead(request.Email) : request.IsTest;
            //original.HasOptedOutOfEmail = request.HasOptedOutOfEmail;
            //original.LeadGUID = request.LeadGUID.HasValue ? request.LeadGUID.Value : original.LeadGUID;
            //original.LegacyGPLeadId = ValidationExtensions.Merge(original.LegacyGPLeadId, request.LegacyGPLeadId);

            return original;
        }


        public LeadSalesforceInfo MergeLeadSalesforceInfoFromUpdate(LeadSalesforceInfo original, SalesforceInfoLead request)
        {
            original.InvalidWrongNumberEmailStrategy = request.InvalidWrongNumberEmailStrategy ?? original.InvalidWrongNumberEmailStrategy;
            original.EmailCommunicationOnly = request.EmailCommunicationOnly ?? original.EmailCommunicationOnly;
            original.StealthAppTotalOutboundDials = request.StealthAppTotalOutboundDials ?? original.StealthAppTotalOutboundDials;
            original.TotalOutboundDialsFromSubStatus = request.TotalOutboundDialsFromSubStatus ?? original.TotalOutboundDialsFromSubStatus;
            original.LastDialOutcome = !String.IsNullOrWhiteSpace(request.LastDialOutcome) ? request.LastDialOutcome : original.LastDialOutcome;
            original.LastCallAge =  request.LastCallAge ?? original.LastCallAge;
            original.ScheduledSMSCount = request.ScheduledSMSCount ?? original.ScheduledSMSCount;
            original.LastCallDate = request.LastCallDate ?? original.LastCallDate;
            original.NextCallTime = request.NextCallTime ?? original.NextCallTime;
            original.PreviousCallTime = request.PreviousCallTime ?? original.PreviousCallTime;
            original.NextScheduledSMSDate = request.NextScheduledSMSDate ?? original.NextScheduledSMSDate;
            original.PreviousScheduledSMSDate = request.PreviousScheduledSMSDate ?? original.PreviousScheduledSMSDate;
            return original;
        }

        public LeadClientInfo MergeLeadClientInfoBaseFromUpdate(LeadClientInfo original, ClientInfoLeadBase request)
        {
            original.Status = !String.IsNullOrWhiteSpace(request.ClientStatus) ? request.ClientStatus : original.Status;
            original.InitialStartTerm = !String.IsNullOrWhiteSpace(request.ClientInitialStartTerm) ? request.ClientInitialStartTerm : original.InitialStartTerm;
            original.ApplicationStartTerm = !String.IsNullOrWhiteSpace(request.ClientApplicationStartTerm) ? request.ClientApplicationStartTerm : original.ApplicationStartTerm;
            original.Notes = !String.IsNullOrWhiteSpace(request.ClientNotes) ? request.ClientNotes : original.Notes;
            original.PipelineNotes = !String.IsNullOrWhiteSpace(request.PipelineNotes) ? request.PipelineNotes : original.PipelineNotes; //SF to GP only
            
            return original;
        }

        public LeadClientInfo MergeLeadClientInfoFromUpdate(LeadClientInfo original, ClientInfoLead request)
        {
            original = MergeLeadClientInfoBaseFromUpdate(original, request);
            original.ApplicationDegreeName = !String.IsNullOrWhiteSpace(request.ClientApplicationDegreeName) ? request.ClientApplicationDegreeName : original.ApplicationDegreeName;
            original.ApplicationDate = request.ClientApplicationDate ?? original.ApplicationDate;
            original.StatusUpdatedDate = request.ClientStatusUpdatedDate ?? original.StatusUpdatedDate;
            original.InterviewDate = request.ClientInterviewDate ?? original.InterviewDate;
            original.StartDate = request.ClientStartDate ?? original.StartDate;
            original.CustomFields = request.CustomFields ?? original.CustomFields;
            original.StartReceivedDate = request.ClientStartReceivedDate ?? original.StartReceivedDate;
            original.ContactDate = request.ClientContactDate ?? original.ContactDate;
            original.EnrollDate = request.ClientEnrollDate ?? original.EnrollDate;
            original.ApplicationStartDate = request.ClientApplicationStartDate ?? original.ApplicationStartDate;
            original.AdmitDate = request.ClientAdmitDate ?? original.AdmitDate;
            original.AppointmentDate = request.ClientAppointmentDate ?? original.AppointmentDate;
            original.QualifiedDate = request.ClientQualifiedDate ?? original.QualifiedDate;
            original.GraduateDate = request.ClientGraduateDate ?? original.GraduateDate;
            original.FirstTermPersistDate = request.ClientFirstTermPersistDate ?? original.FirstTermPersistDate;
            original.ApplicationSubmittedDate = request.ClientApplicationSubmittedDate ?? original.ApplicationSubmittedDate;
            original.DepositDate = request.ClientDepositDate ?? original.DepositDate;
            original.ApplicationDeniedDate = request.ClientApplicationDeniedDate ?? original.ApplicationDeniedDate;
            original.CompletedApplicationChecklistItems = request.CompletedApplicationChecklistItems ?? original.CompletedApplicationChecklistItems;
            original.PendingApplicationChecklistItems = request.PendingApplicationChecklistItems ?? original.PendingApplicationChecklistItems;
            original.PipelineNotes = request.PipelineNotes ?? original.PipelineNotes;

            return original;
        }


        public Lead MapEMSLeadFromISLead(VW_ISLead source)
        {
            int? EducationLevelId = null;
            StringComparison comparer = StringComparison.OrdinalIgnoreCase;
            Dictionary<string, string> additionalQuestions = DataUtils.ParseAdditionalFieldsXml(source.AdditionalFields);

            if (Int32.TryParse(source.EducationLevelId, out int EducationLevelIdParse))
            {
                EducationLevelId = EducationLevelIdParse;
            }

            int currentLeadSourceType = (int)Constants.LeadSourceType.LandingPage;
            string leadSourceTypeString = additionalQuestions.FirstOrDefault(x => String.Equals(x.Key, "LeadSourceType", comparer)).Value;
            if (!string.IsNullOrEmpty(leadSourceTypeString))
            {
                int leadSourceType = 0;
                if (int.TryParse(leadSourceTypeString, out leadSourceType)) {
                    switch (leadSourceType)
                    {
                        case 5:
                            currentLeadSourceType = (int)Constants.LeadSourceType.Microsite;
                            break;
                    }
                }
            }



            int defaultStatusId = Constants.STATUS_NEW_ID;
            int? defaultSubStatusId = null;

            if (serviceRules.CheckIfMigratedInstitutionMCC(source.InstitutionId))
            {
                defaultStatusId = Constants.MCC_STATUS_NEW_ID;
                defaultSubStatusId = Constants.MCC_SUBSTATUS_NEW_ID;
            }

            Lead result = new Lead()
            {
                Address1 = ValidationExtensions.SetValueToNullIfEmpty(source.Address1),
                Address2 = ValidationExtensions.SetValueToNullIfEmpty(source.Address2),
                //AnticipatedStartDate = source.AnticipatedStartDate, TBD
                City = ValidationExtensions.SetValueToNullIfEmpty(source.City),
                CountryCode = ValidationExtensions.SetValueToNullIfEmpty(source.CountryCode),
                PostalCode = ValidationExtensions.SetValueToNullIfEmpty(source.ZipCode),
                LeadStatusId = defaultStatusId,
                LeadSubStatusId = defaultSubStatusId,
                LeadStateId = Constants.STATE_OPEN_ID,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                EducationLevelId = EducationLevelId,
                Email = ValidationExtensions.SetValueToNullIfEmpty(source.EmailAddress),
                InstitutionId = source.InstitutionId,
                FirstName = ValidationExtensions.SetValueToNullIfEmpty(source.FirstName),
                LastName = ValidationExtensions.SetValueToNullIfEmpty(source.LastName),
                LeadGUID = Guid.NewGuid(),
                LeadSourceTypeId = currentLeadSourceType,
                Phone1 = DataUtils.ParsePhone(source.Phone1),
                Phone2 = DataUtils.ParsePhone(source.Phone2),
                ISProgramProductId = source.ProgramProductId,
                ISLeadId = source.LeadID,
                StateProvince = ValidationExtensions.SetValueToNullIfEmpty(source.StateProvince),
                utm_campaign = ValidationExtensions.SetValueToNullIfEmpty(source.utm_campaign),
                utm_channel = ValidationExtensions.SetValueToNullIfEmpty(source.utm_channel),
                utm_vendor = ValidationExtensions.SetValueToNullIfEmpty(source.utm_vendor),
                ExternalId = ValidationExtensions.SetValueToNullIfEmpty(source.ExternalId),
                IsTest = ValidationExtensions.IsTestLead(source.EmailAddress),
                MediaPlanItemId = source.MediaPlanItemId,
                CPL = source.CPL
            };
            return result;
        }

        public Lead MapEMSLeadFromSalesforceInsert(SalesforceLead source)
        {
            Lead result = new Lead()
            {
                Address1 = ValidationExtensions.SetValueToNullIfEmpty(source.Address1),
                Address2 = ValidationExtensions.SetValueToNullIfEmpty(source.Address2),
                City = ValidationExtensions.SetValueToNullIfEmpty(source.City),
                CountryCode = ValidationExtensions.SetValueToNullIfEmpty(source.CountryCode),
                PostalCode = ValidationExtensions.SetValueToNullIfEmpty(source.PostalCode),
                EducationLevelId = source.EducationLevelId,
                Email = ValidationExtensions.SetValueToNullIfEmpty(source.Email),
                InstitutionId = serviceRules.GetInstitutionIdFromSalesforceToken(source.InstitutionSalesforceId),
                FirstName = ValidationExtensions.SetValueToNullIfEmpty(source.FirstName),
                LastName = ValidationExtensions.SetValueToNullIfEmpty(source.LastName),
                LeadSourceTypeId = (int)Constants.LeadSourceType.SalesForce,
                LeadGUID = source.LeadGUID.HasValue ? source.LeadGUID.Value : Guid.NewGuid(),
                Phone1 = ValidationExtensions.SetValueToNullIfEmpty(DataUtils.ParsePhone(source.Phone1)),
                Phone2 = ValidationExtensions.SetValueToNullIfEmpty(DataUtils.ParsePhone(source.Phone2)),
                ISProgramProductId = serviceRules.GetProgramProductIdFromSalesforceToken(source.ProgramProductSalesforceId),
                StateProvince = ValidationExtensions.SetValueToNullIfEmpty(source.StateProvince),
                LeadStatusId = serviceRules.GetLeadStatusId(source.CurrentStatus),
                LeadSubStatusId = serviceRules.GetLeadSubStatusId(source.CurrentSubStatus),
                LeadStateId = serviceRules.GetLeadStateId(source.CurrentState),
                SalesforceId = ValidationExtensions.SetValueToNullIfEmpty(source.SalesforceId),
                Notes = ValidationExtensions.SetValueToNullIfEmpty(HtmlRemoval.StripHtmlTags(source.Notes)),
                IsTest = !source.IsTest ? ValidationExtensions.IsTestLead(source.Email) : source.IsTest,
                HasOptedOutOfEmail = source.HasOptedOutOfEmail,
                LeadOwner = ValidationExtensions.SetValueToNullIfEmpty(source.LeadOwner),
                ClosedReasonCode = ValidationExtensions.SetValueToNullIfEmpty(source.ClosedReasonCode),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                SFUpdatedDate = DateTime.Now,
                ExternalId = ValidationExtensions.SetValueToNullIfEmpty(source.ExternalId),
                LegacyGPLeadId = !string.IsNullOrWhiteSpace(source.LegacyGPLeadId) ? source.LegacyGPLeadId : null,
                StartTerm = ValidationExtensions.SetValueToNullIfEmpty(source.StartTerm),
                utm_campaign = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Campaign),
                utm_channel = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Channel),
                utm_vendor = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Vendor),
                IsStealthApp = source.IsStealthApp,
                RewarmingIndicator = ValidationExtensions.SetValueToNullIfEmpty(source.RewarmingIndicator),
                AppDialOutcome = ValidationExtensions.SetValueToNullIfEmpty(source.AppDialOutcome),
                RewarmingDialOutcome = ValidationExtensions.SetValueToNullIfEmpty(source.RewarmingDialOutcome),
                RewarmingContactAttempts = source.RewarmingContactAttempts,
                HasOptedOutOfSMS = source.HasOptedOutOfSMS,
                Timezone = ValidationExtensions.SetValueToNullIfEmpty(source.Timezone),
                TimezoneOffset = ValidationExtensions.SetValueToNullIfEmpty(source.TimezoneOffset)

            };

            return result;
        }


        /// <summary>
        /// Maps request allowed properties to be changed for update purposes, alters original object
        /// </summary>
        /// <param name="original"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Lead MergeEMSLeadFromSalesforceUpdate(Lead original, SalesforceLead request)
        {
            original.Address1 = ValidationExtensions.SetValueToNullIfEmpty(request.Address1);
            original.Address2 = ValidationExtensions.SetValueToNullIfEmpty(request.Address2);
            original.City = ValidationExtensions.SetValueToNullIfEmpty(request.City);
            original.CountryCode = ValidationExtensions.SetValueToNullIfEmpty(request.CountryCode);
            original.PostalCode = ValidationExtensions.SetValueToNullIfEmpty(request.PostalCode);
            original.LeadStatusId = serviceRules.GetLeadStatusId(request.CurrentStatus);
            original.LeadSubStatusId = serviceRules.GetLeadSubStatusId(request.CurrentSubStatus);
            original.LeadStateId = serviceRules.GetLeadStateId(request.CurrentState);
            original.EducationLevelId = request.EducationLevelId;
            original.Email = request.Email;
            original.InstitutionId = serviceRules.GetInstitutionIdFromSalesforceToken(request.InstitutionSalesforceId);
            original.FirstName = ValidationExtensions.SetValueToNullIfEmpty(request.FirstName);
            original.Email = request.Email;
            original.LastName = request.LastName;
            original.Phone1 = DataUtils.ParsePhone(request.Phone1);
            original.Phone2 = DataUtils.ParsePhone(request.Phone2);
            original.UpdatedDate = DateTime.Now;
            original.SFUpdatedDate = DateTime.Now;
            original.ISProgramProductId = serviceRules.GetProgramProductIdFromSalesforceToken(request.ProgramProductSalesforceId);
            original.SalesforceId = request.SalesforceId;
            original.StateProvince = ValidationExtensions.SetValueToNullIfEmpty(request.StateProvince);
            original.Notes = ValidationExtensions.SetValueToNullIfEmpty(HtmlRemoval.StripHtmlTags(request.Notes));
            original.IsTest = !request.IsTest ? ValidationExtensions.IsTestLead(request.Email) : request.IsTest;
            original.HasOptedOutOfEmail = request.HasOptedOutOfEmail;
            original.LeadOwner = ValidationExtensions.SetValueToNullIfEmpty(request.LeadOwner);
            original.ClosedReasonCode = ValidationExtensions.SetValueToNullIfEmpty(request.ClosedReasonCode);
            original.ExternalId = (!string.IsNullOrWhiteSpace(original.ExternalId)) ? original.ExternalId : (request.ExternalId == string.Empty) ? null : request.ExternalId;
            original.StartTerm = ValidationExtensions.SetValueToNullIfEmpty(request.StartTerm);
            original.IsStealthApp = request.IsStealthApp;
            original.RewarmingIndicator = ValidationExtensions.SetValueToNullIfEmpty(request.RewarmingIndicator);
            original.AppDialOutcome = ValidationExtensions.SetValueToNullIfEmpty(request.AppDialOutcome);
            original.RewarmingDialOutcome = ValidationExtensions.SetValueToNullIfEmpty(request.RewarmingDialOutcome);
            original.RewarmingContactAttempts = request.RewarmingContactAttempts;
            original.HasOptedOutOfSMS = request.HasOptedOutOfSMS;
            original.utm_channel = string.IsNullOrEmpty(original.utm_channel) ? ValidationExtensions.SetValueToNullIfEmpty(request.UTM_Channel) : original.utm_channel;
            return original;
        }

        public List<LeadActivity> MapSalesforceTaskActivity(List<SalesforceQueryTask> tasks, long leadStatusHistoryId, long leadId)
        {
            List<LeadActivity> Result = new List<LeadActivity>();

            foreach (var task in tasks)
            {
                Result.Add(new LeadActivity()
                {
                    CreatedDate = DateTime.Now,
                    LeadActivityTypeId = task.TaskSubtype == "Email" ? (int)Constants.LeadActivityType.Email : (int)Constants.LeadActivityType.Task,
                    LeadStatusHistoryId = leadStatusHistoryId,
                    LeadId = leadId,
                    SalesforceLastModifiedDate = task.LastModifiedDate,
                    JsonData = JsonConvert.SerializeObject(task, Formatting.Indented)
                });
            }

            return Result;
        }

        public List<LeadActivity> MapSalesforceEventActivity(List<SalesforceQueryEvent> events, long leadStatusHistoryId, long leadId)
        {
            List<LeadActivity> Result = new List<LeadActivity>();

            foreach (var evnt in events)
            {
                Result.Add(new LeadActivity()
                {
                    CreatedDate = DateTime.Now,
                    LeadActivityTypeId = (int)Constants.LeadActivityType.Event,
                    LeadStatusHistoryId = leadStatusHistoryId,
                    LeadId = leadId,
                    SalesforceLastModifiedDate = evnt.LastModifiedDate,
                    JsonData = JsonConvert.SerializeObject(evnt, Formatting.Indented)
                });
            }

            return Result;
        }

        public List<LeadActivity> MapSalesforceNoteActivity(List<SalesforceQueryNote> notes, long leadStatusHistoryId, long leadId)
        {
            List<LeadActivity> Result = new List<LeadActivity>();

            foreach (var evnt in notes)
            {
                Result.Add(new LeadActivity()
                {
                    CreatedDate = DateTime.Now,
                    LeadActivityTypeId = (int)Constants.LeadActivityType.Note,
                    LeadStatusHistoryId = leadStatusHistoryId,
                    LeadId = leadId,
                    SalesforceLastModifiedDate = evnt.LastModifiedDate,
                    JsonData = JsonConvert.SerializeObject(evnt, Formatting.Indented)
                });
            }

            return Result;
        }

        #region GPImport

        public Lead MapEMSEMSLeadFromLegacyGPLead(LegacyGPLead source)
        {
            Lead result = new Lead()
            {
                Address1 = ValidationExtensions.SetValueToNullIfEmpty(source.Address1),
                Address2 = ValidationExtensions.SetValueToNullIfEmpty(source.Address2),
                City = ValidationExtensions.SetValueToNullIfEmpty(source.City),
                CountryCode = ValidationExtensions.SetValueToNullIfEmpty(source.CountryCode),
                PostalCode = ValidationExtensions.SetValueToNullIfEmpty(source.PostalCode),
                LeadStatusId = source.LeadStatusId.HasValue ? source.LeadStatusId.Value : Constants.STATUS_NEW_ID,
                LeadSubStatusId = source.LeadSubStatusId,
                LeadStateId = source.LeadStateId.HasValue ? source.LeadStateId.Value : Constants.STATE_OPEN_ID,
                CreatedDate = source.CreatedDate,
                UpdatedDate = source.UpdatedDate,
                EducationLevelId = source.EducationLevelId,
                Email = ValidationExtensions.SetValueToNullIfEmpty(source.Email),
                InstitutionId = source.InstitutionId.HasValue ? source.InstitutionId.Value : 0,
                FirstName = ValidationExtensions.SetValueToNullIfEmpty(source.FirstName),
                LeadGUID = Guid.NewGuid(),
                LegacyGPLeadId = ValidationExtensions.SetValueToNullIfEmpty(source.LegacyGPLeadId),
                LastName = ValidationExtensions.SetValueToNullIfEmpty(source.LastName),
                LeadSourceTypeId = (int)Constants.LeadSourceType.GPImport,
                Phone1 = ValidationExtensions.SetValueToNullIfEmpty(DataUtils.ParsePhone(source.Phone1)),
                Phone2 = ValidationExtensions.SetValueToNullIfEmpty(DataUtils.ParsePhone(source.Phone2)),
                ISProgramProductId = source.ProgramProductId,
                StateProvince = ValidationExtensions.SetValueToNullIfEmpty(source.StateProvince),
                Notes = ValidationExtensions.SetValueToNullIfEmpty(HtmlRemoval.StripHtmlTags(source.Notes)),
                IsTest = !source.IsTest ? ValidationExtensions.IsTestLead(source.Email) : source.IsTest,
                HasOptedOutOfEmail = source.HasOptedOutOfEmail,
                ExternalId = ValidationExtensions.SetValueToNullIfEmpty(source.ExternalId),
                utm_campaign = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Campaign),
                utm_channel = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Channel),
                utm_vendor = ValidationExtensions.SetValueToNullIfEmpty(source.UTM_Vendor),
                ClosedReasonCode = ValidationExtensions.SetValueToNullIfEmpty(source.ClosedReasonCode),
                LeadOwner = ValidationExtensions.SetValueToNullIfEmpty(source.LeadOwner),

            };

            return result;
        }

        /// <summary>
        /// Maps request allowed properties to be changed for update purposes, alters original object
        /// </summary>
        /// <param name="original"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Lead MergeEMSLeadFromLegacyGPLeadUpdate(Lead original, LegacyGPLead request)
        {
            original.Address1 = ValidationExtensions.Update(original.Address1, request.Address1);
            original.Address2 = ValidationExtensions.Update(original.Address2, request.Address2);
            original.City = ValidationExtensions.Update(original.City, request.City);
            // original.ClientStatus = ValidationExtensions.Update(original.ClientStatus, request.ClientStatus);
            original.CountryCode = ValidationExtensions.Update(original.CountryCode, request.CountryCode);
            original.PostalCode = ValidationExtensions.Update(original.PostalCode, request.PostalCode);
            original.LeadStatusId = request.LeadStatusId.HasValue ? request.LeadStatusId.Value : original.LeadStateId;
            original.LeadSubStatusId = request.LeadSubStatusId.HasValue ? request.LeadSubStatusId.Value : original.LeadSubStatusId;
            original.LeadStateId = request.LeadStateId.HasValue ? request.LeadStateId.Value : original.LeadStateId;
            original.EducationLevelId = request.EducationLevelId;
            original.Email = ValidationExtensions.Update(original.Email, request.Email);
            original.InstitutionId = request.InstitutionId.HasValue ? request.InstitutionId.Value : original.InstitutionId;
            original.FirstName = ValidationExtensions.Update(original.FirstName, request.FirstName);
            original.LastName = ValidationExtensions.Update(original.LastName, request.LastName);
            // original.LeadGUID = request.LeadGUID.HasValue ? request.LeadGUID.Value : original.LeadGUID;
            original.LegacyGPLeadId = ValidationExtensions.Update(original.LegacyGPLeadId, request.LegacyGPLeadId);
            original.Phone1 = ValidationExtensions.Update(original.Phone1, DataUtils.ParsePhone(request.Phone1));
            original.Phone2 = ValidationExtensions.Update(original.Phone2, DataUtils.ParsePhone(request.Phone2));
            original.ISProgramProductId = request.ProgramProductId.HasValue ? request.ProgramProductId.Value : original.ISProgramProductId;
            original.StateProvince = ValidationExtensions.Update(original.StateProvince, request.StateProvince);
            original.UpdatedDate = DateTime.Now;
            original.Notes = ValidationExtensions.Update(original.Notes, request.Notes);
            original.IsTest = !request.IsTest ? ValidationExtensions.IsTestLead(request.Email) : request.IsTest;
            original.HasOptedOutOfEmail = request.HasOptedOutOfEmail;
            original.ExternalId = request.ExternalId;
            original.utm_campaign = ValidationExtensions.Update(original.utm_campaign, request.UTM_Campaign);
            original.utm_channel = ValidationExtensions.Update(original.utm_channel, request.UTM_Channel);
            original.utm_vendor = ValidationExtensions.Update(original.utm_vendor, request.UTM_Vendor);
            original.ClosedReasonCode = ValidationExtensions.Update(original.ClosedReasonCode, request.ClosedReasonCode);
            original.LeadOwner = request.LeadOwner;
            return original;
        }


        #endregion
    }
}
