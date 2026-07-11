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
using System.Configuration;
using Microsoft.Azure.EventGrid.Models;
using EDDY.IS.EmsLeadEngine.Entities.EventGrid;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public partial class EmsLeadEngine : EmsLeadEngineBase
    {
        public MultipleResponse ProcessFromDataExchange(ExchangeMultipleLeadProcessRequest request)
        {
            MultipleResponse ResultList = new MultipleResponse()
            {
                TransactionId = request.TransactionId
            };

            foreach (var leadRequest in request.ProcessRequestList)
            {
                try
                {
                    //Insert action
                    if (leadRequest.LeadAction == Constants.ExchangeLeadAction.Insert)
                    {
                        ExchangeInsert(leadRequest, request.TransactionId);
                    }
                    //Update/Upsert action
                    else
                    {
                        ExchangeUpsert(leadRequest, request.TransactionId);
                    }

                    ResultList.Responses.Add(new Response()
                    {
                        TransactionId = leadRequest.SubTransactionId,
                        Success = true,
                        Message = Constants.OK_GENERAL,
                        Code = (int)Constants.ResponseCode.OK
                    });
                }
                catch (Exception ex)
                {
                    LogManager.LogJournalException(request.TransactionId, leadRequest.SubTransactionId, ex, leadRequest);
                    ResultList.Responses.Add(new Response()
                    {
                        TransactionId = leadRequest.SubTransactionId,
                        Success = false,
                        Message = string.Format(Constants.ERR_GENERAL, ex.ToString()),
                        Code = (int)Constants.ResponseCode.ERROR
                    });

                    string action = leadRequest.LeadAction == Constants.ExchangeLeadAction.Update ? Entities.Constants.EMSTRANSACTION_UPDATE
                        : leadRequest.LeadAction == Constants.ExchangeLeadAction.Insert ? Entities.Constants.EMSTRANSACTION_INSERT
                        : Entities.Constants.EMSTRANSACTION_UPSERT;

                    DataService.CreateLeadTransaction(new LeadTransaction()
                    {
                        Action = action,
                        Success = false,
                        TransactionDate = DateTime.Now,
                        TransactionId = request.TransactionId,
                        SubTransactionId = leadRequest.SubTransactionId
                    });

                }
            }

            return ResultList;
        }

        private void ExchangeInsert(ExchangeLeadProcessRequest leadRequest, Guid transactionId)
        {
            if (leadRequest.LeadUniqueKey.HasValue)
            {
                var EMSLead = DataService.GetEMSLead(leadRequest.LeadUniqueKey.Value, leadRequest.Lead.ExternalId, leadRequest.Lead.LeadGUID, leadRequest.Lead.LegacyGPLeadId, leadRequest.Lead.Email, leadRequest.Lead.ProgramProductId, leadRequest.Lead.ISLeadId, leadRequest.Lead.Phone1, leadRequest.Lead.EMSInstitutionId);

                if (EMSLead != null)
                {
                    throw new Exception($"ProcessFromDataExchange Lead Insert requested but Lead already exists based on LeadUniqueKey.");
                }
            }

            var mappedEMSLead = Mappings.MapEMSLeadFromExchange(leadRequest.Lead);

            // check if lead is a duplicate
            bool isDuplicate = DataService.IsEMSLeadDuplicate(mappedEMSLead);

            if (isDuplicate && string.IsNullOrEmpty(mappedEMSLead.ClosedReasonCode))
            {
                mappedEMSLead.LeadStateId = Constants.STATE_CLOSED_ID;
                mappedEMSLead.ClosedReasonCode = Constants.STATE_CLOSEDREASON_CODE;
            }

            bool sendToSF = (leadRequest.RemoveFromSF != true) && serviceRules.InstitutionAllowsContactCenterServices(leadRequest.Lead.EMSInstitutionId);
            bool programAllowsSF = serviceRules.ProgramAllowsContactCenterServices(leadRequest.Lead.ProgramProductId);
            bool programStateAllowsSF = DataService.AllowSyncToSFBasedOnProgramStateRules(mappedEMSLead);           

            if (sendToSF)
            {
                if( ( !programAllowsSF ) ||
                    ( leadRequest.Lead.EMSInstitutionId == 113 && leadRequest.Lead.EducationLevelId == (int)Constants.Educationlevel.HaventCompletedHighSchool )
                  )
                {
                    sendToSF = false;
                    mappedEMSLead.LeadStateId = Constants.STATE_CLOSED_ID;
                    mappedEMSLead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_SCHOOLWORKINGLEAD;
                }
            }



            //Save EMS Lead
            long EMSLeadId = DataService.CreateEMSLead(mappedEMSLead, transactionId, leadRequest.SubTransactionId, false);
            LogManager.LogJournalInfo(transactionId, leadRequest.SubTransactionId, $"ProcessFromDataExchange EMSLeadId inserted, EMSLeadId={EMSLeadId}");

            // Save LeadClientInfo
            LeadClientInfo leadClientInfo = CreateLeadClientInfo(leadRequest.Lead, EMSLeadId, "ProcessFromDataExchange", leadRequest.SubTransactionId);

            // Save lead marketing attribution
            CreateLeadMarketingAttribution(leadRequest.Lead, EMSLeadId, "ProcessFromDataExchange", leadRequest.SubTransactionId);

            //Create new lead history blob with additional questions if passed
            if (leadRequest.AdditionalQuestions != null && leadRequest.AdditionalQuestions.Count > 0)
            {
                LeadHistory leadHistory = CreateNewLeadHistory(EMSLeadId, Constants.LeadSourceType.DataExchange, leadRequest.AdditionalQuestions);
                DataService.CreateEMSLeadHistory(leadHistory, Constants.LeadHistoryAction.Create);
            }

            //check to see if institution is postup enabled
            bool isPostUpEnabled = DataService.IsInstitutionPostUpEnabled(leadRequest.Lead.EMSInstitutionId);

            if (isPostUpEnabled)
            {
                //create master lead list record and send to postup
                SendPostUpRequest(EMSLeadId, "ProcessFromDataExchange", transactionId);
            }

            //PHEA-120 utm_source = google - all leads would deliver to PEA and not EDDY SF, EDDYCoach = False || ig or fb leads => EDDYCoach = true & deliver to Eddy SF
            bool routeBySource = DataService.CanRoutePHEALeadBySource(leadRequest.Lead.EMSInstitutionId, leadRequest.Lead.lma_utm_source);

            if (leadRequest.RouteToActiveCampaign
                 && serviceRules.InstituionAllowsActiveCampaign(leadRequest.Lead.EMSInstitutionId)
                 && serviceRules.ProgramsWithActiveCampaign(leadRequest.Lead.ProgramProductId)
                 && programStateAllowsSF
                 && routeBySource)
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
                            EMSLeadId = EMSLeadId
                        }
                    }
                };

                SendEventGridRequest(EMSLeadId, "ProcessFromDataExchange", evlist, transactionId);
            }

            if (routeBySource && sendToSF && programAllowsSF && programStateAllowsSF)
            {
                InsertLeadRequest azureRequest = new InsertLeadRequest()
                {
                    EMSLeadId = EMSLeadId,
                    RemoveFromFive9 = leadRequest.RemoveFromFive9,
                    RouteToFive9 = true, //TBD
                    TransactionId = transactionId,
                    SubTransactionId = leadRequest.SubTransactionId,
                    RequestType = Constants.LeadServiceRequestType.DataExchange,
                    StealthAppIndicator = leadRequest.StealthAppIndicator
                };
               
                if (!isDuplicate)
                {
                    SendAzureLeadInsertRequest("ProcessFromDataExchange", azureRequest);
                }
            }

            if (mappedEMSLead != null)
            {
                APIRequestConverter apiConverter = new APIRequestConverter();
                mappedEMSLead.LeadId = EMSLeadId;
                apiConverter.Process(mappedEMSLead, "ProcessFromDataExchange", leadClientInfo);

            }
        }
        
        private void ExchangeUpsert(ExchangeLeadProcessRequest leadRequest, Guid transactionId)
        {
            //LeadUniqueKey required for Update/Upsert comparison
            if (!leadRequest.LeadUniqueKey.HasValue)
            {
                throw new Exception($"ProcessFromDataExchange Lead update/Upsert requested but LeadUniqueKey was not provided.");
            }

            var EMSLead = DataService.GetEMSLead(leadRequest.LeadUniqueKey.Value, leadRequest.Lead.ExternalId, leadRequest.Lead.LeadGUID, leadRequest.Lead.LegacyGPLeadId, leadRequest.Lead.Email, leadRequest.Lead.ProgramProductId, leadRequest.Lead.ISLeadId, leadRequest.Lead.Phone1, leadRequest.Lead.EMSInstitutionId);
            long EMSLeadId = 0;
            LeadClientInfo leadClientInfo = null;
            Lead mappedEMSLead = null;
            bool hasSFId = true;

            //Update EMSLead if found by any given token
            if (EMSLead != null)
            {
                hasSFId = EMSLead.SalesforceId == null ? false : true;
                EMSLeadId = EMSLead.LeadId;
                mappedEMSLead = Mappings.MergeEMSLeadFromExchangeUpdate(EMSLead, leadRequest.Lead);
                leadRequest.Lead.ProgramProductId = leadRequest.Lead.ProgramProductId ?? mappedEMSLead.ISProgramProductId;
                DataService.UpdateEMSLead(mappedEMSLead, transactionId, leadRequest.SubTransactionId, EMSLead.ClosedReasonCode);
                LogManager.LogJournalInfo(transactionId, leadRequest.SubTransactionId, $"ProcessFromDataExchange EMSLead found by ExternalId {leadRequest.Lead.ExternalId}, leadGUID {leadRequest.Lead.LeadGUID}, LegacyGPLeadId {leadRequest.Lead.LegacyGPLeadId} and was updated, EMSLeadId={EMSLead.LeadId}");
                
                // Update LeadClientInfo
                leadClientInfo = UpsertLeadClientInfo(leadRequest.Lead, EMSLeadId, "ProcessFromDataExchange", leadRequest.SubTransactionId);

                //Create new lead history blob with additional questions if passed
                if (leadRequest.AdditionalQuestions != null && leadRequest.AdditionalQuestions.Count > 0)
                {
                    LeadHistory leadHistory = CreateNewLeadHistory(EMSLead.LeadId, Constants.LeadSourceType.DataExchange, leadRequest.AdditionalQuestions);
                    DataService.CreateEMSLeadHistory(leadHistory, Constants.LeadHistoryAction.Update);
                }
            }
            else if (leadRequest.LeadAction == Constants.ExchangeLeadAction.Upsert)
            {
                //Save EMS Lead
                mappedEMSLead = Mappings.MapEMSLeadFromExchange(leadRequest.Lead);
                EMSLeadId = DataService.CreateEMSLead(mappedEMSLead, transactionId, leadRequest.SubTransactionId);
                LogManager.LogJournalInfo(transactionId, leadRequest.SubTransactionId, $"ProcessFromDataExchange EMSLeadId inserted, EMSLeadId={EMSLeadId}");

                // Save LeadClientInfo
                leadClientInfo = CreateLeadClientInfo(leadRequest.Lead, EMSLeadId, "ProcessFromDataExchange", leadRequest.SubTransactionId);

                //Create new lead history blob with additional questions if passed
                if (leadRequest.AdditionalQuestions != null && leadRequest.AdditionalQuestions.Count > 0)
                {
                    LeadHistory leadHistory = CreateNewLeadHistory(EMSLeadId, Constants.LeadSourceType.DataExchange, leadRequest.AdditionalQuestions);
                    DataService.CreateEMSLeadHistory(leadHistory, Constants.LeadHistoryAction.Create);
                }

            }
            else //requested update but not found
            {
                throw new Exception($"ProcessFromDataExchange Lead update requested but lead was not found using ExternalId {leadRequest.Lead.ExternalId}, leadGUID {leadRequest.Lead.LeadGUID}, LegacyGPLeadId {leadRequest.Lead.LegacyGPLeadId}");
            }

            bool programStateAllowsSF = DataService.AllowSyncToSFBasedOnProgramStateRules(mappedEMSLead);

            if (leadRequest.RouteToActiveCampaign
                && serviceRules.InstituionAllowsActiveCampaign(leadRequest.Lead.EMSInstitutionId)
                && serviceRules.ProgramsWithActiveCampaign(leadRequest.Lead.ProgramProductId)
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
                            EMSLeadId = EMSLeadId
                        }
                    }
                };

                SendEventGridRequest(EMSLeadId, "ProcessFromDataExchange", evlist, transactionId);
            }


            //Updated to not send update request if no salesforce id
            if (serviceRules.InstitutionAllowsContactCenterServices(leadRequest.Lead.EMSInstitutionId) && hasSFId)
            {
                UpdateLeadRequest azureRequest = new UpdateLeadRequest()
                {
                    EMSLeadId = EMSLeadId,
                    RemoveFromFive9 = leadRequest.RemoveFromFive9,
                    RouteToFive9 = null, //TBD
                    TransactionId = transactionId,
                    SubTransactionId = leadRequest.SubTransactionId,
                    RequestType = Constants.LeadServiceRequestType.DataExchange,
                };
                SendAzureLeadUpdateRequest("ProcessFromDataExchange", azureRequest);
            }

            if (mappedEMSLead != null)
            {
                APIRequestConverter apiConverter = new APIRequestConverter();
                mappedEMSLead.LeadId = EMSLeadId;
                apiConverter.Process(mappedEMSLead, "ProcessFromDataExchange", leadClientInfo);
            }
            
        }
    }
}
