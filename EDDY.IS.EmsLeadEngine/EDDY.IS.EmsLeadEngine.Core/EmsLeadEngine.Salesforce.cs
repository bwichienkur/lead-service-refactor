using EDDY.IS.EmsLeadEngine.Core.DataModel;
using EDDY.IS.EmsLeadEngine.Core.Properties;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Entities.Common;
using EDDY.IS.EmsLeadEngine.Entities.AzureFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using EDDY.IS.EmsLeadEngine.Entities.EventGrid;
using System.Threading;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public partial class EmsLeadEngine : EmsLeadEngineBase
    {        
        public Response UpdateFromSalesforceById(SalesforceLeadUpdateRequest request)
        {
            Response Result;

            var emsLead = DataService.GetEMSLeadFromSalesforceId(request.SalesforceId);
            if (emsLead == null)
            {
                //sometimes salesforce syncs comes faster than lead save
                Thread.Sleep(1000);
                emsLead = DataService.GetEMSLeadFromSalesforceId(request.SalesforceId);
                if (emsLead == null)
                {
                    throw new Exception($"UpdateFromSalesforceById Lead not found exception SalesforceId={request.SalesforceId}");
                }
            }

            Result = new Response()
            {
                TransactionId = request.TransactionId,
                Success = true,
                Message = Constants.OK_GENERAL,
                Code = (int)Constants.ResponseCode.OK
            };

            //Async process
            Task.Run(() => UpdateFromSalesforceByIdAsync(request, emsLead, Result));

            return Result;
        }

        private void UpdateFromSalesforceByIdAsync(SalesforceLeadUpdateRequest request, Lead emsLead, Response Result)
        {
            try
            {
                SalesforceProcessor salesforceProcessor = new SalesforceProcessor();
                SalesforceLead salesforceLead = salesforceProcessor.GetLead(request.SalesforceId, request.TransactionId);
                LogManager.LogJournalInfo(request.TransactionId, $"UpdateFromSalesforceByIdAsync, Data Retrieved From Salesforce", salesforceLead);

                Lead mappedEMSLead = Mappings.MergeEMSLeadFromSalesforceUpdate(emsLead, salesforceLead);
                //Update EMS Lead
                long leadStatusHistoryId = DataService.UpdateEMSLead(mappedEMSLead, request.TransactionId, null, salesforceLead.ClosedReasonCode);

                LogManager.LogJournalInfo(request.TransactionId, $"UpdateFromSalesforceByIdAsync, Updated EMSLeadId = {emsLead.LeadId}");

                LeadSalesforceInfo lsi = null;

                //Upsert Lead Salesforce Info

                lsi = UpsertLeadSalesforceInfo(new SalesforceInfoLead() { 
                    InvalidWrongNumberEmailStrategy = salesforceLead.InvalidWrongNumberEmailStrategy
                    ,EmailCommunicationOnly = salesforceLead.EmailCommunicationOnly
                    ,StealthAppTotalOutboundDials = salesforceLead.StealthAppTotalOutboundDials
                    ,TotalOutboundDialsFromSubStatus = salesforceLead.TotalOutboundDialsFromSubStatus
                   ,LastDialOutcome = salesforceLead.LastDialOutcome
                   ,LastCallAge = salesforceLead.LastCallAge
                   ,NextCallTime = salesforceLead.NextCallTime
                   ,PreviousCallTime = salesforceLead.PreviousCallTime
                   ,NextScheduledSMSDate = salesforceLead.NextScheduledSMSDate
                   ,PreviousScheduledSMSDate = salesforceLead.PreviousScheduledSMSDate
                   ,ScheduledSMSCount = salesforceLead.ScheduledSMSCount
                   ,LastCallDate = salesforceLead.LastCallDate
                }, emsLead.LeadId, "UpdateFromSalesforceById", request.TransactionId);
                

                LeadClientInfo lci = null;
                bool importLCI = serviceRules.InstitutionRequiresLeadClientInfoImport(emsLead.InstitutionId);
                if (importLCI)
                {
                    //Upsert LeadClientInfo 
                    lci = UpsertLeadClientInfo(new ClientInfoLeadBase()
                    {
                        ClientStatus = salesforceLead.CurrentClientStatus,
                        PipelineNotes = salesforceLead.PipelineNotes
                    }, emsLead.LeadId, "UpdateFromSalesforceById", request.TransactionId);
                }
                else
                {
                    LogManager.LogJournalInfo(request.TransactionId, $"UpdateFromSalesforceByIdAsync, Institution {emsLead.InstitutionId} doesn't require Lead Client Info information from salesforce, skipping.");
                }

                bool programStateAllowsSF = DataService.AllowSyncToSFBasedOnProgramStateRules(mappedEMSLead);

                if (serviceRules.InstituionAllowsActiveCampaign(emsLead.InstitutionId)
                    && serviceRules.ProgramsWithActiveCampaign(emsLead.ISProgramProductId)
                    && programStateAllowsSF)
                {
                    var evlist = new List<EventGridEvent>() {
                    new EventGridEvent() {
                        Topic = "lead",
                        Id = Guid.NewGuid().ToString(),
                        EventType = "EMSLeadUpdated",
                        Subject = "BusinessDivision/EMS",
                        EventTime = DateTime.Now,
                        DataVersion = "1.0",
                        Data = new EventGridLeadData(){
                            EMSLeadId = emsLead.LeadId
                            }
                        }
                    };

                    SendEventGridRequest(emsLead.LeadId, "UpdateFromSalesforceByIdAsync", evlist, request.TransactionId);
                }

                if (serviceRules.InstitutionRequiresLeadActivityImport(emsLead.InstitutionId))
                {
                    LogManager.LogJournalInfo(request.TransactionId, $"UpdateFromSalesforceByIdAsync, Institution {emsLead.InstitutionId} requires Lead Additional information from salesforce, querying data.");

                    //LeadActivityInformation if available
                    //Task
                    string lastModifiedDate = DataService.GetLastActivityModifiedDate(emsLead.LeadId, Constants.LeadActivityType.Task);
                    var tasks = salesforceProcessor.GetTasks(request.SalesforceId, lastModifiedDate, false, request.TransactionId);
                    if (tasks != null && tasks.Count > 0)
                    {
                        DataService.CreateEMSLeadActivities(Mappings.MapSalesforceTaskActivity(tasks, leadStatusHistoryId, emsLead.LeadId));
                    }

                    //Email
                    lastModifiedDate = DataService.GetLastActivityModifiedDate(emsLead.LeadId, Constants.LeadActivityType.Email);
                    var taskEmails = salesforceProcessor.GetTasks(request.SalesforceId, lastModifiedDate, true, request.TransactionId);
                    if (taskEmails != null && taskEmails.Count > 0)
                    {
                        DataService.CreateEMSLeadActivities(Mappings.MapSalesforceTaskActivity(taskEmails, leadStatusHistoryId, emsLead.LeadId));
                    }

                    //Events
                    lastModifiedDate = DataService.GetLastActivityModifiedDate(emsLead.LeadId, Constants.LeadActivityType.Event);
                    var events = salesforceProcessor.GetEvents(request.SalesforceId, lastModifiedDate, request.TransactionId);
                    if (events != null && events.Count > 0)
                    {
                        DataService.CreateEMSLeadActivities(Mappings.MapSalesforceEventActivity(events, leadStatusHistoryId, emsLead.LeadId));
                    }

                    //Notes
                    lastModifiedDate = DataService.GetLastActivityModifiedDate(emsLead.LeadId, Constants.LeadActivityType.Note);
                    var notes = salesforceProcessor.GetNotes(request.SalesforceId, lastModifiedDate, request.TransactionId);
                    if (notes != null && notes.Count > 0)
                    {
                        DataService.CreateEMSLeadActivities(Mappings.MapSalesforceNoteActivity(notes, leadStatusHistoryId, emsLead.LeadId));
                    }
                }
                else
                {
                    LogManager.LogJournalInfo(request.TransactionId, $"UpdateFromSalesforceByIdAsync, Institution {emsLead.InstitutionId} doesn't require Lead Additional information from salesforce, skipping.");
                }

                LogManager.LogJournalInfo(request.TransactionId, "UpdateFromSalesforceByIdAsync Result", Result); // Logged in async result

                List<CustomEventInstitutionMapping> customEventRules = serviceRules.GetCustomEventInstitutionMappings(emsLead.InstitutionId);

                if (mappedEMSLead != null && (importLCI || customEventRules.Where(r => r.FieldUpdatedFromSalesforce).Any()))
                {
                    APIRequestConverter apiConverter = new APIRequestConverter();
                    mappedEMSLead.LeadId = emsLead.LeadId;
                    apiConverter.Process(mappedEMSLead, "UpdateFromSalesforceById", lci, customEventRules);

                }
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(request.TransactionId, ex, request);
            }

        }

        public Response CreateFromSalesforce(SalesforceLeadCreateRequest request)
        {
            Response Result;

            Result = new Response()
            {
                TransactionId = request.TransactionId,
                Success = true,
                Message = Constants.OK_GENERAL,
                Code = (int)Constants.ResponseCode.OK
            };

            //Async process
            Task.Run(() => CreateFromSalesforceAsync(request, Result));

            return Result;
        }


        private void CreateFromSalesforceAsync(SalesforceLeadCreateRequest request, Response result)
        {
            try
            {
                var lead = Mappings.MapEMSLeadFromSalesforceInsert(request.Lead);
                //Update EMS Lead
                long emsLeadId = DataService.CreateEMSLead(lead, request.TransactionId, null);
                LogManager.LogJournalInfo(request.TransactionId, $"CreateFromSalesforceAsync, EMSLeadId={emsLeadId}");

                //Create new lead history blob with additional questions if passed
                if (request.AdditionalQuestions != null && request.AdditionalQuestions.Count > 0)
                {
                    LeadHistory leadHistory = CreateNewLeadHistory(emsLeadId, Constants.LeadSourceType.SalesForce, request.AdditionalQuestions);
                    DataService.CreateEMSLeadHistory(leadHistory, Constants.LeadHistoryAction.Create);
                }

                if (serviceRules.InstitutionRequiresLeadClientInfoImport(lead.InstitutionId))
                {
                    //Upsert LeadClientInfo 
                    UpsertLeadClientInfo(new ClientInfoLeadBase()
                    {
                        ClientStatus = request.Lead.CurrentClientStatus,
                        PipelineNotes = request.Lead.PipelineNotes
                    }, emsLeadId, "UpdateFromSalesforceById", request.TransactionId);
                }
                else
                {
                    LogManager.LogJournalInfo(request.TransactionId, $"CreateFromSalesforceByIdAsync, Institution {lead.InstitutionId} doesn't require Lead Client Info information from salesforce, skipping.");
                }

                //check if we need to send to postup
                bool isPostUpEnabled = DataService.IsInstitutionPostUpEnabled(lead.InstitutionId);

                if (isPostUpEnabled)
                {
                    //create master lead list record and send to postup
                    SendPostUpRequest(emsLeadId, "CreateFromSalesforceAsync", request.TransactionId);
                }
                bool programStateAllowsSF = DataService.AllowSyncToSFBasedOnProgramStateRules(lead);

                if (serviceRules.InstituionAllowsActiveCampaign(lead.InstitutionId) 
                    && serviceRules.ProgramsWithActiveCampaign(lead.ISProgramProductId)
                    && programStateAllowsSF)
                {
                    var evlist = new List<EventGridEvent>() {
                    new EventGridEvent() {
                        Topic = "lead",
                        Id = Guid.NewGuid().ToString(),
                        EventType = "EMSLeadCreated",
                        Subject = "BusinessDivision/EMS",
                        EventTime = DateTime.Now,
                        DataVersion = "1.0",
                        Data = new EventGridLeadData(){
                            EMSLeadId = emsLeadId
                            }
                        }
                    };

                    SendEventGridRequest(emsLeadId, "CreateFromSalesforceAsync", evlist, request.TransactionId);
                }

                LogManager.LogJournalInfo(request.TransactionId, "CreateFromSalesforceAsync Result", result); // Logged in async result
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(request.TransactionId, ex, request);
            }

        }

        public bool Unsubscribe(Guid transactionId, string email)
        {
            bool Result = true;

            var emsLeads = DataService.GetLeadIDFromEmail(email);
            if (emsLeads == null || emsLeads.Count == 0)
            {
                throw new Exception($"Unsubscribe Lead not found exception email={email}");
            }

            foreach (var leadId in emsLeads)
            {
                Guid subTransactionId = Guid.NewGuid();
                DataService.UnsubscribeLead(leadId);
                LogManager.LogJournalInfo(transactionId, subTransactionId, $"Unsubscribe Lead updated on EMS Database, EMSLeadId = {leadId}");

                int EMSInstitutionId = DataService.GetInstitutionIDFromLeadID(leadId);

                if (serviceRules.InstitutionAllowsContactCenterServices(EMSInstitutionId))
                {
                    UpdateLeadRequest azureRequest = new UpdateLeadRequest()
                    {
                        EMSLeadId = leadId,
                        TransactionId = transactionId,
                        SubTransactionId = subTransactionId,
                        RequestType = Constants.LeadServiceRequestType.Unsubscribe
                    };

                    SendAzureLeadUpdateRequest("Unsubscribe", azureRequest);
                    LogManager.LogJournalInfo(transactionId, subTransactionId, $"Unsubscribe Lead Sent to Azure function, EMSLeadId = {leadId}");
                }
            }


            return Result;
        }

        public string UnsubscribeBySalesforceId(Guid transactionId, string salesforceId)
        {
            string Result = "Your unsubscribe request has been processed and you will be removed from all related email communications within 48 hours.";

            //Update to accommodate both the 15 and 18 character versions of salesforce id
            string formattedSalesforceId = salesforceId;

            if (salesforceId.Length == 15)
                formattedSalesforceId = ConvertTo18CharacterSFID(formattedSalesforceId);

            var leadId = DataService.GetLeadIdFromSalesforceId(formattedSalesforceId);
            if (leadId == 0 )
            {
                throw new Exception($"UnsubscribeBySalesforceId Lead not found exception SalesforceId={formattedSalesforceId}");
            }

            DataService.UnsubscribeLead(leadId);
            LogManager.LogJournalInfo(transactionId, $"UnsubscribeBySalesforceId Lead updated on EMS Database, EMSLeadId = {leadId}");

            int EMSInstitutionId = DataService.GetInstitutionIDFromLeadID(leadId);

            if (serviceRules.InstitutionAllowsContactCenterServices(EMSInstitutionId))
            {
                UpdateLeadRequest azureRequest = new UpdateLeadRequest()
                {
                    EMSLeadId = leadId,
                    TransactionId = transactionId,
                    RequestType = Constants.LeadServiceRequestType.Unsubscribe
                };

                SendAzureLeadUpdateRequest("UnsubscribeBySalesforceId", azureRequest);
                LogManager.LogJournalInfo(transactionId, $"UnsubscribeBySalesforceId Lead Sent to Azure function, EMSLeadId = {leadId}");
            }


            return Result;
        }

        public string UnsubscribeByEMSLeadId(Guid transactionId, int emsLeadId)
        {
            string Result = "Your unsubscribe request has been processed and you will be removed from all related email communications within 48 hours.";

            if (emsLeadId == 0)
                throw new Exception($"UnsubscribeByEMSLeadId Lead not found exception EMSLeadId={emsLeadId}");

            DataService.UnsubscribeLead(emsLeadId);
            LogManager.LogJournalInfo(transactionId, $"UnsubscribeByEMSLeadId Lead updated on EMS Database, EMSLeadId = {emsLeadId}");
            var lead = DataService.GetEMSLead(emsLeadId);

            if (serviceRules.InstitutionAllowsContactCenterServices(lead.InstitutionId) && !string.IsNullOrEmpty(lead.SalesforceId))
            {
                UpdateLeadRequest azureRequest = new UpdateLeadRequest()
                {
                    EMSLeadId = emsLeadId,
                    TransactionId = transactionId,
                    RequestType = Constants.LeadServiceRequestType.Unsubscribe
                };

                SendAzureLeadUpdateRequest("UnsubscribeByEMSLeadId", azureRequest);
                LogManager.LogJournalInfo(transactionId, $"UnsubscribeByEMSLeadId Lead Sent to Azure function, EMSLeadId = {emsLeadId}");
            }

            ////need to also unsubscribe from active campaign
            //if (serviceRules.InstituionAllowsActiveCampaign(lead.InstitutionId))
            //{
            //    var evlist = new List<EventGridEvent>() {
            //        new EventGridEvent() {
            //            Topic = "emailmarketingintegrator",
            //            Id = Guid.NewGuid().ToString(),
            //            EventType = "UnsubscribeByEMSLeadId",
            //            Subject = "BusinessDivision/EMS",
            //            EventTime = DateTime.Now,
            //            DataVersion = "1.0",
            //            Data = new EventGridLeadData(){
            //                EMSLeadId = emsLeadId
            //            }
            //        }
            //    };

            //    SendEventGridRequest(emsLeadId, "UnsubscribeByEMSLeadId", evlist, transactionId);
            //}

            return Result;
        }

        public string UpdateEmailStrategyExhausted(Guid transactionId,string salesforceId)
        {
            string Result = "Successfully Sent Email Strategy Exhausted";

            var lead = DataService.GetEMSLeadFromSalesforceId(salesforceId);

            if (serviceRules.InstitutionAllowsContactCenterServices(lead.InstitutionId))
            {
                UpdateLeadRequest azureRequest = new UpdateLeadRequest()
                {
                    EMSLeadId = lead.LeadId,
                    TransactionId = transactionId,
                    RequestType = Constants.LeadServiceRequestType.EmailStrategyExhausted
                };

                SendAzureLeadUpdateRequest("UpdateEmailStrategyExhausted", azureRequest);
                LogManager.LogJournalInfo(transactionId, $"UpdateEmailStrategyExhausted Lead Sent to Azure function, EMSLeadId = {lead.LeadId}");
            }

            return Result;
        }

        private string ConvertTo18CharacterSFID(string salesforceId)
        {
            Dictionary<string, char> BinaryIdLookup = new Dictionary<string, char>
            {
                {"00000", 'A'}, {"00001", 'B'}, {"00010", 'C'}, {"00011", 'D'}, {"00100", 'E'},
                {"00101", 'F'}, {"00110", 'G'}, {"00111", 'H'}, {"01000", 'I'}, {"01001", 'J'},
                {"01010", 'K'}, {"01011", 'L'}, {"01100", 'M'}, {"01101", 'N'}, {"01110", 'O'},
                {"01111", 'P'}, {"10000", 'Q'}, {"10001", 'R'}, {"10010", 'S'}, {"10011", 'T'},
                {"10100", 'U'}, {"10101", 'V'}, {"10110", 'W'}, {"10111", 'X'}, {"11000", 'Y'},
                {"11001", 'Z'}, {"11010", '0'}, {"11011", '1'}, {"11100", '2'}, {"11101", '3'},
                {"11110", '4'}, {"11111", '5'}
            };

            //Separate the salesforceId into 3 sections
            List<string> sections = new List<string>() {
                salesforceId.Substring(0,5),
                salesforceId.Substring(5,5),
                salesforceId.Substring(10,5)
            };

            StringBuilder sb = new StringBuilder();
            string suffix = string.Empty;

            foreach (var section in sections)
            {
                //Clear the string builder for each section
                sb.Clear();
                //Reverse each section
                var reverse = section.Reverse().ToList();
                //For each character in the reverse string, if it is a capitalized character, replace with 1. Otherwise 0
                reverse.ForEach(r => sb.Append(Char.IsUpper(r) ? '1' : '0'));
                //Convert the binary into character using lookup
                suffix += BinaryIdLookup[sb.ToString()];
            }

            return salesforceId + suffix;
        }
    }
}
