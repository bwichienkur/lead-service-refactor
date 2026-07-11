using EDDY.IS.EmsLeadEngine.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EDDY.IS.EmsLeadEngine.Entities.Constants;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    public class ServiceRules
    {
        public int GetLeadStateId(string leadStateName)
        {
            int Result = 1; //Active by default since SF field is nullable
            if (!string.IsNullOrWhiteSpace(leadStateName))
            {
                if (CacheDataService.LeadStateDictionary.ContainsKey(leadStateName.ToLowerInvariant()))
                {
                    Result = CacheDataService.LeadStateDictionary[leadStateName.ToLowerInvariant()].LeadStateId;
                }
                else
                {
                    throw new Exception($"LeadState not found or invalid State provided: {leadStateName}");
                }
            }

            return Result;
        }
        public int GetLeadStatusId(string leadStatusName)
        {
            int Result = -1;
            if (!string.IsNullOrWhiteSpace(leadStatusName) && CacheDataService.LeadStatusDictionary.ContainsKey(leadStatusName.ToLowerInvariant()))
            {
                Result = CacheDataService.LeadStatusDictionary[leadStatusName.ToLowerInvariant()].LeadStatusId;
            }
            else
            {
                throw new Exception($"Status not found or invalid status provided: {leadStatusName}");
            }

            return Result;
        }

        public int? GetLeadSubStatusId(string leadSubStatusName)
        {
            int? Result = null;

            if (!string.IsNullOrWhiteSpace(leadSubStatusName) && CacheDataService.LeadSubStatusDictionary.ContainsKey(leadSubStatusName.ToLowerInvariant()))
            {
                Result = CacheDataService.LeadSubStatusDictionary[leadSubStatusName.ToLowerInvariant()].LeadSubStatusId;
            }

            return Result;
        }

        public int GetInstitutionIdFromSalesforceToken(string salesforceToken)
        {
            int Result = -1;
            if (!string.IsNullOrWhiteSpace(salesforceToken) && CacheDataService.SalesforceInstitutionMappingDictionary.ContainsKey(salesforceToken))
            {
                Result = CacheDataService.SalesforceInstitutionMappingDictionary[salesforceToken];
            }
            else
            {
                throw new Exception($"Institution not found or invalid salesforceToken provided: {salesforceToken}");
            }

            return Result;
        }

        public int? GetProgramProductIdFromSalesforceToken(string salesforceToken)
        {
            int? Result = null;
            if (!string.IsNullOrWhiteSpace(salesforceToken) && CacheDataService.SalesforceProgramProductMappingDictionary.ContainsKey(salesforceToken))
            {
                Result = CacheDataService.SalesforceProgramProductMappingDictionary[salesforceToken];
            }

            return Result;
        }

        public bool InstitutionAllowsContactCenterServices(int institutionId)
        {
            return CacheDataService.InstitutionsWithContactCenterServices.Any(x => x.InstitutionId == institutionId);
        }
        public bool ProgramAllowsContactCenterServices(int? programProductId)
        {
            if (programProductId == null)
                return true;
            return !CacheDataService.ProgramsWithoutContactCenterServices.Any(p => p == programProductId);
        }        
        public bool ProgramsWithActiveCampaign(int? programProductId)
        {
            if (programProductId == null)
                return false;
            return CacheDataService.ProgramsWithActiveCampaign.Any(p => p == programProductId);
        }
        public bool InstituionAllowsActiveCampaign(int institutionId)
        {
            return CacheDataService.InstitutionsWithActiveCampaign.Any(x => x.InstitutionId == institutionId);
        }
        public bool InstituionConfiguredWithProgramStateSFSync(int institutionId)
        {
            return CacheDataService.InstitutionsWithProgramStateSFSync.Any(x => x == institutionId);
        }

        public bool InstitutionAllowsRoutingToSalesFource(VW_ISLead nexusLead)
        {
            bool allowsRoutingToSF = true;

            //Checking to see if has campus type rules that are applicable to SF routing
            if (CacheDataService.InstitutionsWithContactCenterServices.Any(x => x.InstitutionId == nexusLead.InstitutionId
                                                          && x.ContractEntityId != null
                                                          && x.ContractEntityId == 5))
            {
                string ruleCampusTypeStr = CacheDataService.InstitutionsWithContactCenterServices.FirstOrDefault(x => x.InstitutionId == nexusLead.InstitutionId
                                                                                                && x.ContractEntityId != null
                                                                                                && x.ContractEntityId == 5).Value;
                if (nexusLead.CampusTypeId != Int32.Parse(ruleCampusTypeStr))
                    allowsRoutingToSF = false;
            }

            return allowsRoutingToSF;
        }

        public bool InstitutionRequiresLeadActivityImport(int institutionId)
        {
            return CacheDataService.InstitutionsWithLeadActivityImport.Contains(institutionId);
        }

        public bool InstitutionRequiresLeadClientInfoImport(int institutionId)
        {
            return CacheDataService.InstitutionsWithLeadClientInfoImport.Contains(institutionId);
        }

        public bool IsTestLead(Lead emsLead)
        {
            return emsLead.IsTest;
        }

        public List<CustomEventInstitutionMapping> GetCustomEventInstitutionMappings(int institutionId)
        {
            return CacheDataService.CustomEventInstitutionMappings.Where(x => x.EMSInstitutionId == institutionId).ToList();
        }
        public bool CheckIfMigratedInstitutionMCC(int institutionId)
        {
            return CacheDataService.InstitutionsThatMigratedMCC.Where(x => x == institutionId).Any();
        }        
    }
}
