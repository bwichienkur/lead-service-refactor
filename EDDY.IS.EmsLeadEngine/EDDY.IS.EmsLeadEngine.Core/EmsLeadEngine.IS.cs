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
using System.Xml;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Azure.EventGrid.Models;
using EDDY.IS.EmsLeadEngine.Entities.EventGrid;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public partial class EmsLeadEngine : EmsLeadEngineBase
    {

        public Response CreateFromIS(ISLeadCreateRequest request)
        {
            var Result = new Response()
            {
                TransactionId = request.TransactionId,
                Success = true,
                Message = Constants.OK_GENERAL,
                Code = (int)Constants.ResponseCode.OK
            };

            //Task.Run(() => CreateFromISAsync(request));
            CreateFromISAsync(request);

            return Result;
        }

        private void CreateFromISAsync(ISLeadCreateRequest request)
        {
            try
            {
                foreach (int leadId in request.ISLeadIds)
                {
                    CreateFromIS(leadId, request.TransactionId);
                }
            }
            catch(Exception ex)
            {
                LogManager.LogJournalException(request.TransactionId, ex, request);
            }
        }

        private void CreateFromIS(int leadId, Guid transactionId)
        {
            try
            {
                VW_ProgramMapping programMapping = null;
                //Retrieve Nexus Lead
                var nexusLead = DataService.GetISLead(leadId);
                if (nexusLead == null)
                {
                    throw new Exception($"CreateFromIS Lead not found exception ISLeadID={leadId}");
                }

                bool initialLeadValidationFailed = nexusLead.InitialLeadValidationFailed == "1";
                bool isSpam = nexusLead.InitialLeadValidationFailedReason == "Spam - Spam";
                bool MarkSpamAsScreened = bool.Parse(ConfigurationManager.AppSettings["MarkSpamAsScreened"]);
                bool isSchoolWorking = false;
                string[] meteorLearningSchools = ConfigurationManager.AppSettings["MeteorLearningInstitutions"].ToString().Split(',');

                // check if lead is a duplicate
                bool isDuplicate = DataService.IsEMSLeadDuplicate(Mappings.MapEMSLeadFromISLead(nexusLead));

                //Institution allows Contact Center Services                
                bool hasContactCenter = serviceRules.InstitutionAllowsContactCenterServices(nexusLead.InstitutionId);

                //If has rules for campus type that affects SF routing, check against thpose 
                bool allowsRoutingToSalesForce = serviceRules.InstitutionAllowsRoutingToSalesFource(nexusLead);
                
                var lead = Mappings.MapEMSLeadFromISLead(nexusLead);

                //If test lea
                bool isTestLead = serviceRules.IsTestLead(lead);

                Dictionary<string, string> additionalFields = DataUtils.ParseAdditionalFieldsXml(nexusLead.AdditionalFields);

                //Requested by Kim and Erick to close state if they don't have a contact center.
                if (!hasContactCenter)
                {
                    lead.LeadStateId = Constants.STATE_CLOSED_ID;
                    lead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_MARKETINGONLY;
                }

                //Allows routing to SF based on presence of a campus rule that has match the institution's campus type
                if (!allowsRoutingToSalesForce)
                {
                    lead.LeadStateId = Constants.STATE_CLOSED_ID;
                    lead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_MARKETINGONLY;
                }

                //Close lead as duplicate if it has a contact center and there already exists a duplicate lead
                if (hasContactCenter && allowsRoutingToSalesForce && isDuplicate)
                {
                    lead.LeadStateId = Constants.STATE_CLOSED_ID;
                    lead.ClosedReasonCode = Constants.STATE_CLOSEDREASON_CODE;
                }

                //Frontier lead should close as school working based on certain values from additional fields
                if (lead.InstitutionId == 189)
                {
                    if (additionalFields.ContainsKey("LicensedRN"))
                    {
                        if (additionalFields["LicensedRN"] == "No")
                        {
                            isSchoolWorking = true;
                        }
                    }

                    if (additionalFields.ContainsKey("HighestLevelofEducationCompleted"))
                    {
                        if (additionalFields["HighestLevelofEducationCompleted"] == "AssociatesDegree")
                        {
                            isSchoolWorking = true;
                        }
                    }

                    if (additionalFields.ContainsKey("RNBachelorsInAnyField"))
                    {
                        if (additionalFields["RNBachelorsInAnyField"] == "No")
                        {
                            isSchoolWorking = true;
                        }
                    }

                    if (additionalFields.ContainsKey("RNAlsoCertifiedField"))
                    {
                        if (additionalFields["RNAlsoCertifiedField"] == "No")
                        {
                            isSchoolWorking = true;
                        }
                    }

                    if (isSchoolWorking)
                    {
                        lead.LeadStateId = Constants.STATE_CLOSED_ID;
                        lead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_SCHOOLWORKINGLEAD;
                    }
                }

                //Allows routing to SF based on presence of a campus rule that has match the institution's campus type
                if (isTestLead)
                {
                    lead.LeadStateId = Constants.STATE_CLOSED_ID;
                    lead.ClosedReasonCode = Constants.TEST_LEAD;
                }

                //Allowed initial failed leads to be saved on ems.Lead table
                if (initialLeadValidationFailed)
                {
                    if (isSpam)
                    {
                        if (MarkSpamAsScreened)
                        {
                            lead.LeadStateId = Constants.STATE_CLOSED_ID;
                            lead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_MARKETINGONLYSCREENEDLEAD;
                        }
                    }
                    else
                    {
                        lead.LeadStateId = Constants.STATE_CLOSED_ID;
                        lead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_MARKETINGONLYSCREENEDLEAD;
                    }
                }

                if (additionalFields.ContainsKey("DoNotSendToSF"))
                {
                    if (additionalFields["DoNotSendToSF"] == "True")                   
                        allowsRoutingToSalesForce = false;                   
                }

                //Add meteor phone/email logic
                if (meteorLearningSchools.Contains(lead.InstitutionId.ToString()) && !isDuplicate)
                {
                    bool isValidLead = ValidateMeteorLearningEmailPhone(lead);
                    if (!isValidLead)
                    {
                        allowsRoutingToSalesForce = false;
                        lead.LeadStateId = Constants.STATE_CLOSED_ID;
                        lead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_MARKETINGONLYSCREENEDLEAD;
                    }
                    else 
                    {
                        if (!string.IsNullOrEmpty(lead.Phone1))
                        {
                            //check to see if it is a duplicate lead by phone
                            bool isDuplicateByPhone = DataService.IsEMSLeadDuplicateByPhone(Mappings.MapEMSLeadFromISLead(nexusLead));
                            if (isDuplicateByPhone)
                            {
                                allowsRoutingToSalesForce = false;
                                lead.LeadStateId = Constants.STATE_CLOSED_ID;
                                lead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_DUPLICATE;
                            }
                        }
                    }
                }
               
                bool programStateAllowsSF = DataService.AllowSyncToSFBasedOnProgramStateRules(lead); //MUOHIO-86            
                bool sendToSF = hasContactCenter && allowsRoutingToSalesForce && !isDuplicate && !initialLeadValidationFailed && !isTestLead && !isSchoolWorking;
                bool programAllowsSF = serviceRules.ProgramAllowsContactCenterServices(lead.ISProgramProductId);

                if (sendToSF)
                {
                    if ( (!programAllowsSF) ||
                         (lead.InstitutionId == 113 && lead.EducationLevelId == (int)Constants.Educationlevel.HaventCompletedHighSchool)
                       )
                    {
                        sendToSF = false;
                        lead.LeadStateId = Constants.STATE_CLOSED_ID;
                        lead.ClosedReasonCode = Constants.CLOSED_REASON_CODE_SCHOOLWORKINGLEAD;
                    }
                }

                
                //Save EMS Lead
                long EMSLeadId = DataService.CreateEMSLead(lead, transactionId, null);
                LogManager.LogJournalInfo(transactionId, $"CreateFromIS EMSLeadId saved, EMSLeadId={EMSLeadId}, hasContactCenter={hasContactCenter}, allowsRoutingToSalesForce={allowsRoutingToSalesForce}, IsDuplicate={isDuplicate}, InitialLeadValidationFailed={initialLeadValidationFailed}, IsTestLead={isTestLead}");

                var (leadClientInfo, leadMarketingAttribution) =
                             ProcessISLeadAdditionalFields(nexusLead, EMSLeadId, transactionId, additionalFields);
                
                string utmSource = string.Empty;
                additionalFields.TryGetValue("utm_source", out utmSource); //get value from AdditionalQuestions
                if (string.IsNullOrEmpty(utmSource))
                {
                    utmSource = leadMarketingAttribution.utm_source;
                }
                
                bool routeBySource = DataService.CanRoutePHEALeadBySource(lead.InstitutionId, utmSource);
                
                if (serviceRules.InstituionAllowsActiveCampaign(lead.InstitutionId)
                    && !isTestLead && !isSchoolWorking
                    && serviceRules.ProgramsWithActiveCampaign(lead.ISProgramProductId)
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

                    SendEventGridRequest(EMSLeadId, "CreateFromIS", evlist, transactionId);
                }

                if (sendToSF && programAllowsSF && programStateAllowsSF && routeBySource)
                {
                    InsertLeadRequest azureRequest = new InsertLeadRequest()
                    {
                        EMSLeadId = EMSLeadId,
                        RemoveFromFive9 = false,
                        RouteToFive9 = true, //TBD
                        TransactionId = transactionId,
                        RequestType = Constants.LeadServiceRequestType.LandingPage
                    };

                   SendAzureLeadInsertRequest("CreateFromIS", azureRequest);
                }

                if (lead != null)
                {
                    APIRequestConverter apiConverter = new APIRequestConverter();
                    lead.LeadId = EMSLeadId;
                    apiConverter.Process(lead, "CreateFromIS", leadClientInfo);

                }
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(transactionId, ex, $"ISLeadId={leadId}");
            }
        }

        private (LeadClientInfo LeadClientInfo, LeadMarketingAttribution LeadMarketingAttribution) ProcessISLeadAdditionalFields(VW_ISLead nexusLead, long emsLeadId, Guid transactionId, Dictionary<string, string> additionalFields)
        {
            CreateEMSLeadHistoryFromISLeadAdditionalFields(additionalFields, nexusLead, emsLeadId);
            LeadClientInfo lci = CreateLeadClientInfoFromISLeadAdditionalFields(additionalFields, emsLeadId, transactionId, nexusLead.InstitutionId);
            var lma = CreateEMSLeadMarketingAttributionFromISLeadAdditionalFields(additionalFields, emsLeadId, nexusLead.EMSContractName);
            return (lci, lma);
        }
        private LeadMarketingAttribution CreateEMSLeadMarketingAttributionFromISLeadAdditionalFields(Dictionary<string, string> additionalFields, long emsLeadId, string contractName)
        {
            LeadMarketingAttribution leadMarketingAttribution = CreateNewLeadMarketingAttribution(emsLeadId, additionalFields);
            if (contractName != null && (contractName.Contains("Inbound") || contractName.Contains("SEO") || contractName.Contains("Organic")))
                leadMarketingAttribution.utm_medium = "Organic";

            DataService.CreateLeadMarketingAttribution(leadMarketingAttribution);
            return leadMarketingAttribution;
        }
        private void CreateEMSLeadHistoryFromISLeadAdditionalFields(Dictionary<string, string> additionalFields, VW_ISLead nexusLead, long emsLeadId)
        {
            if (!string.IsNullOrWhiteSpace(nexusLead.MethodOfContact))
            {
                additionalFields.Add(nameof(nexusLead.MethodOfContact), nexusLead.MethodOfContact);
            }
            var leadHistory = CreateNewLeadHistory(emsLeadId, Constants.LeadSourceType.LandingPage, additionalFields);
            DataService.CreateEMSLeadHistory(leadHistory, Constants.LeadHistoryAction.Create);
        }

        private LeadClientInfo CreateLeadClientInfoFromEntryTermInISLeadAdditionalFields(Dictionary<string, string> additionalFields, long emsLeadId, Guid transactionId)
        {
            LeadClientInfo leadClientInfo = null;
            if (additionalFields.TryGetValue("EntryTerm", out string initialStartTerm))
            {
                leadClientInfo = new LeadClientInfo
                {
                    InitialStartTerm = initialStartTerm,
                    LeadId = emsLeadId
                };

                CreateLeadClientInfo(leadClientInfo, nameof(this.CreateFromIS), transactionId);
            }
            return leadClientInfo;
        }

        private LeadClientInfo CreateLeadClientInfoFromISLeadAdditionalFields(Dictionary<string, string> additionalFields, long emsLeadId, Guid transactionId, int institutionId)
        {
            string initialStartTerm;
            string readyToStart;
            string clientNotes;
            string starCampusId;
            string expectedgraduationyear;
            List<int> meteorLearningInstitutions = new List<int>() { 4, 8, 32, 87 };

            bool createLeadClientInfo = false;
            bool isMeteorLearningInstitution = meteorLearningInstitutions.Contains(institutionId);

            if (additionalFields.TryGetValue("EntryTerm", out initialStartTerm))            
                createLeadClientInfo = true;
            
            if (additionalFields.TryGetValue("ready_to_start", out readyToStart))           
                createLeadClientInfo = true;
            
            if (additionalFields.TryGetValue("ClientNotes", out clientNotes))          
                createLeadClientInfo = true;

            if (institutionId == 113 && additionalFields.TryGetValue("expectedgraduationyear", out expectedgraduationyear))
                createLeadClientInfo = true;

            if (institutionId == 154)          
                createLeadClientInfo = true;

            LeadClientInfo leadClientInfo = null;

            if (createLeadClientInfo)
            {
                leadClientInfo = new LeadClientInfo
                {
                    InitialStartTerm = initialStartTerm,
                    Notes = readyToStart == "Apply Now" ? "Application Requested" : null,
                    LeadId = emsLeadId
                };

                if (isMeteorLearningInstitution)
                    leadClientInfo.Notes = clientNotes;

                if (institutionId == 113 && additionalFields.TryGetValue("expectedgraduationyear", out expectedgraduationyear))
                    leadClientInfo.Notes = expectedgraduationyear;

                if (institutionId == 154)
                {
                    if (additionalFields.TryGetValue("starscampusid", out starCampusId))
                    {
                        string campusName = null;
                        switch (starCampusId)
                        {
                            case "1":
                                campusName = "Scarborough";
                                break;
                            case "2":
                                campusName = "Bangor";
                                break;
                            case "5":
                                campusName = "Auburn";
                                break;
                            default:
                                break;
                        }

                        leadClientInfo.Notes = campusName;
                    }
                }


                CreateLeadClientInfo(leadClientInfo, nameof(this.CreateFromIS), transactionId);
            }
            return leadClientInfo;
        }


        public bool ValidateMeteorLearningEmailPhone(Lead lead)
        {
            bool isPhoneValid = true;
            bool isEmailValid = true;

            string formattedPhone = DataUtils.ParsePhone(lead.Phone1);

            if (!String.IsNullOrEmpty(formattedPhone) && formattedPhone.Length != 10)
            {
                isPhoneValid = false;
            }

            if (!String.IsNullOrEmpty(lead.Email) &&
                (!lead.Email.Contains("@") ||
                lead.Email.StartsWith("@") ||
                lead.Email.EndsWith(".") ||
                lead.Email.Contains("@.")))
            {
                isEmailValid = false;
            }

            return (isPhoneValid || isEmailValid);
        }
    }
}
