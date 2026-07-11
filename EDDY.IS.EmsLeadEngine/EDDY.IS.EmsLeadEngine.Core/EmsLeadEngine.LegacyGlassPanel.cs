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

namespace EDDY.IS.EmsLeadEngine.Core
{
    public partial class EmsLeadEngine : EmsLeadEngineBase
    {
        public MultipleResponse BulkSaveFromlegacyGP(LegacyGPBulkLeadSaveRequest request)
        {

            MultipleResponse resultList = new MultipleResponse()
            {
                TransactionId = request.TransactionId
            };

            foreach (LegacyGPLead lead in request.Leads)
            {
                resultList.Responses.Add(SaveLegacyGPLead(lead, request.TransactionId));
            }

            return resultList;
        }

        public Response SaveLegacyGPLead(LegacyGPLead lead, Guid transactionId)
        {
            Response result = null;
            try
            {
                long emsLeadId = 0;
                string sfLeadId = string.Empty;
                Lead existingLead = DataService.GetEMSLead(Constants.ExchangeLeadUniqueKey.LegacyGPLeadId, null, null, lead.LegacyGPLeadId, null, null, null, null, null);
                if (existingLead == null)
                {
                    //Insert EMS Lead
                    emsLeadId = DataService.CreateEMSLead(Mappings.MapEMSEMSLeadFromLegacyGPLead(lead), transactionId, lead.TransactionId);
                    LogManager.LogJournalInfo(lead.TransactionId, lead.TransactionId, $"CreateFromGPImport, EMSLeadId={emsLeadId}", lead);

                    // Save LeadClientInfo
                    CreateLeadClientInfo(lead, emsLeadId, "CreateFromGPImport", lead.TransactionId);

                    if (!string.IsNullOrWhiteSpace(lead.PreferredMethodOfContact))
                    {
                        Dictionary<string, string> additionalQuestions = new Dictionary<string, string>();
                        additionalQuestions.Add("MethodOfContact", lead.PreferredMethodOfContact);
                        LeadHistory leadHistory = CreateNewLeadHistory(emsLeadId, Constants.LeadSourceType.GPImport, additionalQuestions);
                        DataService.CreateEMSLeadHistory(leadHistory, Constants.LeadHistoryAction.Create);
                    }

                }
                else
                {
                    //Update EMS Lead
                    DataService.UpdateEMSLead(Mappings.MergeEMSLeadFromLegacyGPLeadUpdate(existingLead, lead), transactionId, lead.TransactionId, null);
                    emsLeadId = existingLead.LeadId;
                    sfLeadId = existingLead.SalesforceId;
                    LogManager.LogJournalInfo(transactionId, lead.TransactionId, $"UpdateFromGPImport, EMSLeadId={emsLeadId}", lead);

                    // Update LeadClientInfo
                    UpsertLeadClientInfo(lead, emsLeadId, "UpdateFromGPImport", lead.TransactionId);

                    if (!string.IsNullOrWhiteSpace(lead.PreferredMethodOfContact))
                    {
                        Dictionary<string, string> additionalQuestions = new Dictionary<string, string>();
                        additionalQuestions.Add("MethodOfContact", lead.PreferredMethodOfContact);
                        LeadHistory leadHistory = CreateNewLeadHistory(emsLeadId, Constants.LeadSourceType.GPImport, additionalQuestions);
                        DataService.CreateEMSLeadHistory(leadHistory, Constants.LeadHistoryAction.Update);
                    }
                }
                if (emsLeadId > 0)
                {
                    // No Institution No go
                    if (lead.InstitutionId.HasValue)
                    {
                        //check if we need to send to postup
                        bool isPostUpEnabled = DataService.IsInstitutionPostUpEnabled(lead.InstitutionId.Value);

                        if (isPostUpEnabled && lead.LeadStateId.HasValue && lead.LeadStateId.Value == 1)
                        {
                            //create master lead list record and send to postup
                            SendPostUpRequest(emsLeadId, "CreateFromGPImport", transactionId);
                        }

                        if (serviceRules.InstitutionAllowsContactCenterServices(lead.InstitutionId.Value) 
                            && serviceRules.ProgramAllowsContactCenterServices(lead.ProgramProductId))
                        {
                            bool routeToFive9 = (lead.LeadStateId.HasValue) ? (lead.LeadStateId.Value == 1) : false;
                            if (String.IsNullOrWhiteSpace(sfLeadId))
                            {
                                var mappedLead = Mappings.MapEMSEMSLeadFromLegacyGPLead(lead);
                                bool programStateAllowsSF = DataService.AllowSyncToSFBasedOnProgramStateRules(mappedLead);

                                if (programStateAllowsSF)
                                {
                                    InsertLeadRequest azureRequest = new InsertLeadRequest()
                                    {
                                        EMSLeadId = emsLeadId,
                                        RemoveFromFive9 = false,
                                        RouteToFive9 = routeToFive9, //TBD
                                        TransactionId = transactionId,
                                        SubTransactionId = lead.TransactionId,
                                        RequestType = Constants.LeadServiceRequestType.GPImport
                                    };

                                    SendAzureLeadInsertRequest("GPImport", azureRequest);
                                }
                            }
                            else
                            {
                                UpdateLeadRequest azureRequest = new UpdateLeadRequest()
                                {
                                    EMSLeadId = emsLeadId,
                                    RemoveFromFive9 = false,
                                    RouteToFive9 = routeToFive9, //TBD
                                    TransactionId = transactionId,
                                    SubTransactionId = lead.TransactionId,
                                    Upsert = true,
                                    RequestType = Constants.LeadServiceRequestType.GPImport
                                };
                                SendAzureLeadUpdateRequest("GPImport", azureRequest);
                            }
                        }
                        else
                        {
                            LogManager.LogJournalInfo(transactionId, lead.TransactionId, $"SaveLegacyGPLead Not sending to AzureFunction InstitutionAllowsContactCenterServices({lead.InstitutionId.Value}) is (false) for EMSLeadId={emsLeadId}");
                        }
                    }
                }
                result = new LegacyGPLeadSaveResponse()
                {
                    TransactionId = lead.TransactionId,
                    Success = true,
                    Message = Constants.OK_GENERAL,
                    Code = (int)Constants.ResponseCode.OK
                };
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(lead.TransactionId, ex, lead);
                result = new Response()
                {
                    TransactionId = lead.TransactionId,
                    Success = false,
                    Message = string.Format(Constants.ERR_GENERAL, ex.Message),
                    Code = (int)Constants.ResponseCode.ERROR
                };

                string action = Entities.Constants.EMSTRANSACTION_UPSERT;

                DataService.CreateLeadTransaction(new LeadTransaction()
                {
                    Action = action,
                    Success = false,
                    TransactionDate = DateTime.Now,
                    TransactionId = transactionId,
                    SubTransactionId = lead.TransactionId
                });

            }
            return result;
        }

    }
}
