using EDDY.IS.EmsLeadEngine.Core.Properties;
using EDDY.IS.EMSPostUpServiceClient.com.postup.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    public static class CacheDataService
    {
        private static bool Initialized;
        private static Timer CacheRefreshTimer { get; set; }
        public static Dictionary<string, LeadStatus> LeadStatusDictionary { get; private set; }
        public static Dictionary<string, LeadSubStatus> LeadSubStatusDictionary { get; private set; }
        public static Dictionary<string, LeadState> LeadStateDictionary { get; private set; }
        public static Dictionary<string, int> SalesforceInstitutionMappingDictionary { get; private set; }
        public static Dictionary<string, int> SalesforceProgramProductMappingDictionary { get; private set; }
        public static IList<VW_ContractInstitutionsRules> InstitutionsWithContactCenterServices { get; private set; }
        public static List<int> ProgramsWithoutContactCenterServices { get; private set; }

        public static IList<VW_EmailIntegrationInstitutions> InstitutionsWithActiveCampaign { get; private set; }
        public static HashSet<int> InstitutionsWithLeadActivityImport { get; private set; }
        public static HashSet<int> InstitutionsWithLeadClientInfoImport { get; private set; }
        public static HashSet<int> InstitutionsWithProgramStateSFSync { get; private set; }
        public static IList<CustomEventInstitutionMapping> CustomEventInstitutionMappings { get; private set; }
        public static List<int> InstitutionsThatMigratedMCC { get; set; }
        public static List<int> ProgramsWithActiveCampaign { get; set; }

        public static void Initialize()
        {
            try
            {
                if (!Initialized)
                {
                    LeadStatusDictionary = GetLeadStatusDictionary();
                    LeadSubStatusDictionary = GetLeadSubStatusDictionary();
                    LeadStateDictionary = GetLeadStateDictionary();

                    Dictionary<string, int> _SalesforceInstitutionMappingDictionary;
                    Dictionary<string, int> _SalesforceProgramProductMappingDictionary;
                    GetSalesforceMappingDictionary(out _SalesforceInstitutionMappingDictionary, out _SalesforceProgramProductMappingDictionary);
                    SalesforceInstitutionMappingDictionary = _SalesforceInstitutionMappingDictionary;
                    SalesforceProgramProductMappingDictionary = _SalesforceProgramProductMappingDictionary;

                    InstitutionsWithContactCenterServices = GetInstitutionsWithContactCenterServices();
                    ProgramsWithoutContactCenterServices = GetProgramsWithoutContactCenterServices();
                    InstitutionsWithLeadActivityImport = GetInstitutionsWithLeadActivityImport();
                    InstitutionsWithLeadClientInfoImport = GetInstitutionsWithLeadClientInfoImport();
                    InstitutionsWithActiveCampaign = GetInstitutionsWithActiveCampaign();
                    CustomEventInstitutionMappings = GetCustomEventInstitutionMappings();
                    InstitutionsThatMigratedMCC = GetInstitutionsThatMigratedMCC();
                    ProgramsWithActiveCampaign = GetProgramsWithActiveCampaign();
                    InstitutionsWithProgramStateSFSync = GetInstitutionsWithProgramStateSFSync();

                    CacheRefreshTimer = new Timer(Settings.Default.CacheRefreshMinutes * 60000);
                    CacheRefreshTimer.Elapsed += OnCacheRefreshEvent;
                    CacheRefreshTimer.AutoReset = true;
                    CacheRefreshTimer.Enabled = true;
                }
                Initialized = true;
            }
            catch (Exception ex)
            {
                //Log cache load exception
                LogManager.LogException(ex);
            }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
        }

        private static void OnCacheRefreshEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                var _LeadStatusDictionary = GetLeadStatusDictionary();
                lock (LeadStatusDictionary)
                {
                    LeadStatusDictionary = _LeadStatusDictionary;
                }

                var _LeadSubStatusDictionary = GetLeadSubStatusDictionary();
                lock (LeadSubStatusDictionary)
                {
                    LeadSubStatusDictionary = _LeadSubStatusDictionary;
                }

                var _LeadStateDictionary = GetLeadStateDictionary();
                lock (LeadStateDictionary)
                {
                    LeadStateDictionary = _LeadStateDictionary;
                }

                Dictionary<string, int> _SalesforceInstitutionMappingDictionary;
                Dictionary<string, int> _SalesforceProgramProductMappingDictionary;
                GetSalesforceMappingDictionary(out _SalesforceInstitutionMappingDictionary, out _SalesforceProgramProductMappingDictionary);

                lock (SalesforceInstitutionMappingDictionary)
                {
                    SalesforceInstitutionMappingDictionary = _SalesforceInstitutionMappingDictionary;
                }

                lock (SalesforceProgramProductMappingDictionary)
                {
                    SalesforceProgramProductMappingDictionary = _SalesforceProgramProductMappingDictionary;
                }

                lock (InstitutionsWithContactCenterServices)
                {
                    InstitutionsWithContactCenterServices = GetInstitutionsWithContactCenterServices();
                }
                lock (InstitutionsWithLeadActivityImport)
                {
                    InstitutionsWithLeadActivityImport = GetInstitutionsWithLeadActivityImport();
                }
            }
            catch (Exception ex)
            {
                //Log cache refresh exception
                LogManager.LogException(ex);
            }
        }


        private static Dictionary<string, LeadStatus> GetLeadStatusDictionary()
        {
            Dictionary<string, LeadStatus> Result = new Dictionary<string, LeadStatus>();
            using (EMSModel Context = new EMSModel())
            {
                Result = (from s in Context.LeadStatuses
                          where s.IsEnabled == true
                          select s).ToDictionary(t => t.LeadStatusName.ToLowerInvariant());

            }
            return Result;
        }


        private static Dictionary<string, LeadSubStatus> GetLeadSubStatusDictionary()
        {
            Dictionary<string, LeadSubStatus> Result = new Dictionary<string, LeadSubStatus>();
            using (EMSModel Context = new EMSModel())
            {
                Result = (from s in Context.LeadSubStatuses
                          where s.IsEnabled == true
                          select s).ToDictionary(t => t.LeadSubStatusName.ToLowerInvariant());

            }
            return Result;
        }


        private static Dictionary<string, LeadState> GetLeadStateDictionary()
        {
            Dictionary<string, LeadState> Result = new Dictionary<string, LeadState>();
            using (EMSModel Context = new EMSModel())
            {
                Result = (from s in Context.LeadStates
                          where s.IsEnabled == true
                          select s).ToDictionary(t => t.LeadStateName.ToLowerInvariant());

            }
            return Result;
        }

        private static void GetSalesforceMappingDictionary(out Dictionary<string, int> institutionDictionary, out Dictionary<string, int> programProductDictionary)
        {
            List<VW_SalesforceInstitutionProgramProductMapping> resultList = new List<VW_SalesforceInstitutionProgramProductMapping>();
            institutionDictionary = new Dictionary<string, int>();
            programProductDictionary = new Dictionary<string, int>();

            using (EMSModel Context = new EMSModel())
            {
                resultList = (from s in Context.VW_SalesforceInstitutionProgramProductMappings
                              select s).ToList();
            }

            foreach (var item in resultList)
            {
                if (item.InstitutionSalesforceId != null && !institutionDictionary.ContainsKey(item.InstitutionSalesforceId))
                {
                    institutionDictionary.Add(item.InstitutionSalesforceId, item.InstitutionId);
                }
                if (item.ProgramProductSalesforceId != null && !programProductDictionary.ContainsKey(item.ProgramProductSalesforceId))
                {
                    programProductDictionary.Add(item.ProgramProductSalesforceId, item.ProgramProductId);
                }
            }
        }

        private static IList<VW_ContractInstitutionsRules> GetInstitutionsWithContactCenterServices()
        {
            IList<VW_ContractInstitutionsRules> Result = new List<VW_ContractInstitutionsRules>();
            IList<VW_ContractInstitutionsRules> itemList = new List<VW_ContractInstitutionsRules>();

            using (EMSModel Context = new EMSModel())
            {
                itemList = Context.VW_ContractInstitutionsRules.ToList<VW_ContractInstitutionsRules>();
            }

            Result = new List<VW_ContractInstitutionsRules>(itemList);

            return Result;
        }

        private static List<int> GetProgramsWithoutContactCenterServices()
        {
            List<int> programIdListResult = new List<int>();

            using (EMSModel Context = new EMSModel())
            {
                programIdListResult = Context.VW_ProgramMappings.Where(pm => pm.DoNotRouteToSF == true).Select(pm => pm.ProgramProductId).ToList();
            }

            return programIdListResult;
        }

        private static List<int> GetProgramsWithActiveCampaign()
        {
            List<int> programIdListResult = new List<int>();

            using (EMSModel Context = new EMSModel())
            {
                programIdListResult = Context.VW_ProgramMappings.Where(pm => pm.RouteToActiveCampaign == true).Select(pm => pm.ProgramProductId).ToList();
            }

            return programIdListResult;
        }
        private static HashSet<int> GetInstitutionsWithProgramStateSFSync()
        {
            HashSet<int> Result = new HashSet<int>();
            List<int> itemList = new List<int>();

            using (EMSModel Context = new EMSModel())
            {
                itemList = (from i in Context.Institutions
                            where i.IsEnabled == true &&
                                  i.ProgramStateSyncEnabled == true
                            select i.InstitutionId).ToList<int>();
            }

            Result = new HashSet<int>(itemList);

            return Result;
        }

        private static IList<VW_EmailIntegrationInstitutions> GetInstitutionsWithActiveCampaign()
        {
            IList<VW_EmailIntegrationInstitutions> Result = new List<VW_EmailIntegrationInstitutions>();
            IList<VW_EmailIntegrationInstitutions> itemList = new List<VW_EmailIntegrationInstitutions>();

            using (EMSModel Context = new EMSModel())
            {
                itemList = Context.VW_EmailIntegrationInstitutions.ToList<VW_EmailIntegrationInstitutions>();
            }

            Result = new List<VW_EmailIntegrationInstitutions>(itemList);

            return Result;
        }
        
        private static HashSet<int> GetInstitutionsWithLeadActivityImport()
        {
            HashSet<int> Result = new HashSet<int>();
            List<int> itemList = new List<int>();

            using (EMSModel Context = new EMSModel())
            {
                itemList = (from i in Context.Institutions
                          where i.IsEnabled == true &&
                                i.LeadActivityImport == true
                          select i.InstitutionId).ToList<int>();
            }

            Result = new HashSet<int>(itemList);

            return Result;
        }

        private static HashSet<int> GetInstitutionsWithLeadClientInfoImport()
        {
            HashSet<int> Result = new HashSet<int>();
            List<int> itemList = new List<int>();

            using (EMSModel Context = new EMSModel())
            {
                itemList = (from i in Context.Institutions
                            where i.IsEnabled == true &&
                                  i.LeadClientInfoImport == true
                            select i.InstitutionId).ToList<int>();
            }

            Result = new HashSet<int>(itemList);

            return Result;
        }


        private static IList<CustomEventInstitutionMapping> GetCustomEventInstitutionMappings()
        {
            IList<CustomEventInstitutionMapping> Result = new List<CustomEventInstitutionMapping>();

            using (EMSModel Context = new EMSModel())
            {
                Result = Context.CustomEventInstitutionMappings.ToList<CustomEventInstitutionMapping>();
            }


            return Result;
        }

        private static List<int> GetInstitutionsThatMigratedMCC()
        {
            List<int> Result = new List<int>();

            using (EMSModel Context = new EMSModel())
            {
                Result = (from i in Context.Institutions
                            where i.IsEnabled == true &&
                                  i.MigratedMCC == true
                            select i.InstitutionId).ToList<int>();
            }

            return Result;
        }

    }
}
