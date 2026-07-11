using EDDY.IS.EmsLeadEngine.Core.Extensions;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Entities.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using static EDDY.IS.EmsLeadEngine.Entities.Constants;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    public class EMSDataService
    {
        
        /// <summary>
        /// Gets the current EMS database Server name
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseServerName()
        {
            string Result = string.Empty;

            using (EMSModel Context = new EMSModel())
            {
                Result = Context.Database.Connection.DataSource;
            }

            return Result;
        }

        /// <summary>
        /// Gets the current Marketing database Server name
        /// </summary>
        /// <returns></returns>
        public string GetMarketingDatabaseServerName()
        {
            string Result = string.Empty;

            using (MarketingDataModel Context = new MarketingDataModel())
            {
                Result = Context.Database.Connection.DataSource;
            }

            return Result;
        }

        public Person GetPerson(string email)
        {
            Person Result = null;

            using (EMSModel Context = new EMSModel())
            {
                Result = (from p in Context.People
                          where p.Email == email
                          select p).FirstOrDefault();
            }

            return Result;
        }

        /// <summary>
        /// Merge Person based on e-mail adddress
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Person UpsertPerson(string email, long? emsLeadId = null)
        {
            Person Result;
            using (EMSModel Context = new EMSModel())
            {
                if (emsLeadId.HasValue)
                {
                    Result = Context.Database.SqlQuery<Person>("UpsertPerson @Email, @EMSLeadId", new SqlParameter("@Email", email), new SqlParameter("@EMSLeadId", emsLeadId)).FirstOrDefault();
                }
                else
                {
                    Result = Context.Database.SqlQuery<Person>("UpsertPerson @Email", new SqlParameter("@Email", email)).FirstOrDefault();
                }
            }
            return Result;
        }

        public bool IsEMSLeadDuplicate(Lead lead)
        {
            bool Result = false;
            using (EMSModel Context = new EMSModel())
            {
                var output = new SqlParameter("@Result", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
                Context.Database.ExecuteSqlCommand("IsLeadDuplicate @InstitutionId, @Email, @ProgramProductId, @Result OUTPUT"
                    , new SqlParameter("@InstitutionId", lead.InstitutionId)
                    , new SqlParameter("@Email", lead.Email)
                    , new SqlParameter("@ProgramProductId", lead.ISProgramProductId)
                    , output);

                Result = (bool)output.Value;
            }
            return Result;
        }

        public bool IsEMSLeadDuplicateByPhone(Lead lead)
        {
            bool Result = false;
            using (EMSModel Context = new EMSModel())
            {
                var output = new SqlParameter("@Result", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
                Context.Database.ExecuteSqlCommand("IsLeadDuplicateByPhone @InstitutionId, @Phone, @Result OUTPUT"
                    , new SqlParameter("@InstitutionId", lead.InstitutionId)
                    , new SqlParameter("@Phone", lead.Phone1)
                    , output);

                Result = (bool)output.Value;
            }
            return Result;
        }

        public bool IsInstitutionPostUpEnabled(int institutionId)
        {
            bool Result = false;
            using (EMSModel Context = new EMSModel())
            {
                var inst = Context.Institutions.Where(i => i.InstitutionId == institutionId).FirstOrDefault();
                if (inst.IsPostUpEnabled)
                {
                    Result = true;
                }
            }
            return Result;
        }

        public void LogFBConversionAttemptInfo(long? eMSLeadId, int eventId, string jsonMessage, long? isLeadId)
        {
            using(MarketingDataModel mdm = new MarketingDataModel())
            {
                LeadConversionAPIEvent lcae = new LeadConversionAPIEvent();
                lcae.ConversionAPIEventId = eventId;
                lcae.ConversionAPIMessageStatusId = 1; //new
                lcae.CreatedDate = DateTime.Now;
                lcae.IsEnabled = true;
                lcae.LeadId = isLeadId;
                lcae.EMSLeadId = eMSLeadId;
                lcae.Message = jsonMessage;
                lcae.MessageAttemptCount = 0;
                mdm.LeadConversionAPIEvents.Add(lcae);
                mdm.SaveChanges();
            }
        }
        public void LogFBAudienceAttemptInfo(long? isLeadId, int eventId, string jsonMessage, long? emsLeadId)
        {
            using (MarketingDataModel mdm = new MarketingDataModel())
            {
                AudienceLeadEvent lcae = new AudienceLeadEvent();
                lcae.AudienceEventId = eventId;
                lcae.AudienceMessageStatusId = 1; //new
                lcae.CreatedDate = DateTime.Now;
                lcae.IsEnabled = true;
                lcae.LeadId = isLeadId;
                lcae.Message = jsonMessage;
                lcae.MessageAttemptCount = 0;
                lcae.EMSLeadId = emsLeadId;
                mdm.AudienceLeadEvents.Add(lcae);
                mdm.SaveChanges();
            }
        }

        public void LogGoogleConversionAttemptInfo(long? isLeadId, long? eMSLeadId, int eventId, string jsonMessage)
        {
            using (MarketingDataModel mdm = new MarketingDataModel())
            {
                GLeadConversionAPIEvent lcae = new GLeadConversionAPIEvent();
                lcae.ConversionAPIEventId = eventId;
                lcae.ConversionAPIMessageStatusId = 1; //new
                lcae.CreatedDate = DateTime.Now;
                lcae.IsEnabled = true;
                lcae.LeadId = isLeadId;
                lcae.EMSLeadId = eMSLeadId;
                lcae.Message = jsonMessage;
                lcae.MessageAttemptCount = 0;
                mdm.GLeadConversionAPIEvents.Add(lcae);
                mdm.SaveChanges();
            }
        }

        public void LogGoogleAudienceAttemptInfo(long? isLeadId, long? emsLeadId, int eventId, string jsonMessage)
        {
            using (MarketingDataModel mdm = new MarketingDataModel())
            {
                GAudienceLeadEvent lev = new GAudienceLeadEvent
                {
                    AudienceEventId = eventId,
                    AudienceMessageStatusId = 1, //new
                    CreatedDate = DateTime.Now,
                    IsEnabled = true,
                    LeadId = isLeadId,
                    EMSLeadId = emsLeadId,
                    Message = jsonMessage,
                    MessageAttemptCount = 0
                };

                mdm.GAudienceLeadEvents.Add(lev);
                mdm.SaveChanges();
            }
        }

        /// <summary>
        /// Creates EMSlead, Upserts person and returns EmsLeadID
        /// </summary>
        /// <param name="lead"></param>
        /// <returns></returns>
        public long CreateEMSLead(Lead lead, Guid transactionId, Guid? subTransactionId, bool processDupeLogic = false)
        {
            long Result = 0;
            Person person = UpsertPerson(lead.Email);
            lead.PersonId = person.PersonId;
            bool isDuplicate = false;

            if (processDupeLogic)
            {
                isDuplicate = IsEMSLeadDuplicate(lead);
                if(isDuplicate)
                {
                    lead.LeadStateId = Constants.STATE_CLOSED_ID;
                    lead.ClosedReasonCode = Constants.STATE_CLOSEDREASON_CODE;
                }
            }
            if (lead.Timezone == null || lead.TimezoneOffset == null)
                GetLeadTimezoneData(lead);

            using (EMSModel Context = new EMSModel())
            {
                Context.Leads.Attach(lead);
                Context.Entry(lead).State = EntityState.Added;
                Context.SaveChanges();
                Result = lead.LeadId;

                CreateLeadTransaction(Context, new LeadTransaction()
                {
                    Action = Entities.Constants.EMSTRANSACTION_INSERT,
                    Success = true,
                    TransactionDate = DateTime.Now,
                    LeadId = lead.LeadId,
                    TransactionId = transactionId,
                    SubTransactionId = subTransactionId
                });
            }

            //Lead Status History
            CreateEMSLeadStatusHistory(new LeadStatusHistory()
            {
                LeadStatusId = lead.LeadStatusId,
                LeadSubStatusId = lead.LeadSubStatusId,
                ClosedReasonCode = lead.ClosedReasonCode,
                LeadId = Result,
                CreatedDate = DateTime.Now,
                LeadStateId = lead.LeadStateId,
                LeadOwner = lead.LeadOwner
            });

            return Result;
        }

        public Lead GetEMSLead(long leadId)
        {
            Lead Result = null;

            using (EMSModel Context = new EMSModel())
            {
                Result = (from l in Context.Leads
                          .Include("LeadSourceType")
                          where l.LeadId == leadId
                          select l).FirstOrDefault();
            }

            return Result;
        }

        public Lead GetEMSLeadFromSalesforceId(string salesforceId)
        {
            Lead Result = null;

            using (EMSModel Context = new EMSModel())
            {
                Result = (from l in Context.Leads
                          .Include("LeadSourceType")
                          where l.SalesforceId == salesforceId
                          select l).FirstOrDefault();
            }

            return Result;
        }

        public long GetLeadIdFromSalesforceId(string salesforceId)
        {
            long Result = 0;

            using (EMSModel Context = new EMSModel())
            {
                Result = (from l in Context.Leads
                          where l.SalesforceId == salesforceId
                          select l.LeadId).FirstOrDefault();
            }

            return Result;
        }

        public List<long> GetLeadIDFromEmail(string email)
        {
            List<long> Result = null;

            using (EMSModel Context = new EMSModel())
            {
                Result = (from l in Context.Leads
                          where l.Email == email
                          select l.LeadId).ToList();
            }

            return Result;
        }

        public int GetInstitutionIDFromLeadID(long leadID)
        {
            int Result = 0;

            using (EMSModel Context = new EMSModel())
            {
                Result = (from l in Context.Leads
                          where l.LeadId == leadID
                          select l.InstitutionId).FirstOrDefault();
            }

            return Result;
        }

        
        public string GetLastActivityModifiedDate(long leadID, Constants.LeadActivityType activityType)
        {
            string Result = null;

            using (EMSModel Context = new EMSModel())
            {
                Result = (from a in Context.LeadActivities
                          where a.LeadId == leadID &&
                            a.LeadActivityTypeId == (int)activityType
                          orderby a.LeadActivityId descending
                          select a.SalesforceLastModifiedDate).FirstOrDefault();
            }

            return Result;
        }

        public Lead GetEMSLead(ExchangeLeadUniqueKey key, string externalId, Guid? leadGuid, string legacy, string email, int? programProductId, int? isLeadId, string phone1, int? institutionId)
        {
            Lead Result = null;

            using (EMSModel Context = new EMSModel())
            {
                var query = from l in Context.Leads.Include("LeadSourceType")
                            select l;

                if (key == ExchangeLeadUniqueKey.ExternalId)
                {
                    if(string.IsNullOrWhiteSpace(externalId))
                    {
                        throw new Exception("GetEMSLead ExternalId used on ExchangeLeadUniqueKey but key was not specified.");
                    }
                    query = query.Where(t => t.ExternalId == externalId);
                }
                else if(key == ExchangeLeadUniqueKey.LeadGUID)
                {
                    if (!leadGuid.HasValue || leadGuid.Value==Guid.Empty)
                    {
                        throw new Exception("GetEMSLead LeadGUID used on ExchangeLeadUniqueKey but Guid is null or empty.");
                    }
                    query = query.Where(t => t.LeadGUID == leadGuid.Value);
                }
                else if (key == ExchangeLeadUniqueKey.EmailAddress)
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        throw new Exception("GetEMSLead EmailAddress used on ExchangeLeadUniqueKey but email is null or empty.");
                    }
                    query = query.Where(t => t.Email == email && t.ClosedReasonCode != "Duplicate" && t.InstitutionId == institutionId);
                }
                else if (key == ExchangeLeadUniqueKey.EmailProgram)
                {
                    if (string.IsNullOrWhiteSpace(email) || !programProductId.HasValue)
                    {
                        throw new Exception("GetEMSLead EmailAndProgram used on ExchangeLeadUniqueKey but email/programproductid is null or empty.");
                    }
                    query = query.Where(t => t.Email == email && t.ISProgramProductId == programProductId);
                }
                else if (key == ExchangeLeadUniqueKey.ISLeadId)
                {
                    if (!isLeadId.HasValue)
                    {
                        throw new Exception("GetEMSLead ISLeadId used on ExchangeLeadUniqueKey but ISLeadId is null or empty.");
                    }
                    query = query.Where(t => t.ISLeadId == isLeadId);
                }
                else if (key == ExchangeLeadUniqueKey.Phone1)
                {
                    if (string.IsNullOrWhiteSpace(phone1))
                    {
                        throw new Exception("GetEMSLead Phone1 used on ExchangeLeadUniqueKey but Phone1 is null or empty.");
                    }
                    query = query.Where(t => t.Phone1 == phone1 && t.ClosedReasonCode != "Duplicate" && t.InstitutionId == institutionId);
                }
                else 
                {
                    if (string.IsNullOrWhiteSpace(legacy))
                    {
                        throw new Exception("GetEMSLead LegacyGPLeadId used on ExchangeLeadUniqueKey but legacy value was not specified.");
                    }
                    query = query.Where(t => t.LegacyGPLeadId == legacy);
                }

                Result = query.FirstOrDefault();
            }

            return Result;
        }

        public string GetTrackIdForMediaPlanItem(int mediaPlanItemId)
        {
            string Result = Guid.Empty.ToString();

            using (EMSModel Context = new EMSModel())
            {
                var plan = Context.MediaPlanItems.Find(mediaPlanItemId);
                if (plan != null)
                {
                    Result = plan.CampaignTrackId.ToString();
                }
            }

            return Result;
        }

        public VW_ISLead GetISLead(decimal LeadId)
        {
            VW_ISLead Result = null;

            using (EMSModel Context = new EMSModel())
            {
                Result = (from l in Context.VW_ISLeads
                          where l.LeadID == LeadId
                          select l).FirstOrDefault();
            }

            return Result;
        }

        public bool UnsubscribeLead(long EMSLeadId)
        {
            bool Result = true;

            using (EMSModel Context = new EMSModel())
            {
                Context.Database.ExecuteSqlCommand("UPDATE Lead SET HasOptedOutOfEmail = 1 WHERE LeadId=@EMSLeadId", new SqlParameter("@EMSLeadId", EMSLeadId));
            }

            return Result;
        }
        /// <summary>
        /// Updates a lead and returns the LeadStatusHistoryId
        /// </summary>
        /// <param name="lead"></param>
        /// <param name="transactionId"></param>
        /// <param name="subTransactionId"></param>
        /// <param name="closedReasonCode"></param>
        /// <returns></returns>
        public long UpdateEMSLead(Lead lead, Guid transactionId, Guid? subTransactionId, string closedReasonCode)
        {
            long Result = 0;

            Person person = UpsertPerson(lead.Email, lead.LeadId);
            lead.PersonId = person.PersonId;

            using (EMSModel Context = new EMSModel())
            {
                Context.Leads.Attach(lead);
                Context.Entry(lead).State = EntityState.Modified;
                Context.SaveChanges();
                CreateLeadTransaction(Context, new LeadTransaction() {
                     Action = Entities.Constants.EMSTRANSACTION_UPDATE,
                     Success = true,
                     TransactionDate = DateTime.Now,
                     LeadId = lead.LeadId,
                     TransactionId = transactionId,
                     SubTransactionId = subTransactionId
                });
            }

            //Lead Status History
            Result = CreateEMSLeadStatusHistory(new LeadStatusHistory()
            {
                LeadStatusId = lead.LeadStatusId,
                LeadSubStatusId = lead.LeadSubStatusId,
                LeadId = lead.LeadId,
                CreatedDate = DateTime.Now,
                ClosedReasonCode = closedReasonCode,
                LeadStateId = lead.LeadStateId,
                LeadOwner = lead.LeadOwner
            });

            return Result;
        }

        /// <summary>
        /// Create a new LeadStatusHistory only if the record changes
        /// </summary>
        /// <param name="emsLeadStatusHistory"></param>
        /// <returns></returns>
        public long CreateEMSLeadStatusHistory(LeadStatusHistory emsLeadStatusHistory)
        {
            long Result = 0;

            using (EMSModel Context = new EMSModel())
            {
                var lastHistory = (from lsh in Context.LeadStatusHistories
                                   where lsh.LeadId == emsLeadStatusHistory.LeadId
                                   orderby lsh.LeadStatusHistoryId descending
                                   select lsh).FirstOrDefault();

                if (lastHistory == null
                    || lastHistory.LeadStatusId != emsLeadStatusHistory.LeadStatusId
                    || lastHistory.LeadSubStatusId != emsLeadStatusHistory.LeadSubStatusId
                    || lastHistory.ClosedReasonCode != emsLeadStatusHistory.ClosedReasonCode
                    || lastHistory.LeadStateId != emsLeadStatusHistory.LeadStateId
                    || lastHistory.LeadOwner != emsLeadStatusHistory.LeadOwner
                    )
                {
                    Context.LeadStatusHistories.Attach(emsLeadStatusHistory);
                    Context.Entry(emsLeadStatusHistory).State = EntityState.Added;
                    Context.SaveChanges();
                    Result = emsLeadStatusHistory.LeadStatusHistoryId;

                }
                else
                {
                    Result = lastHistory.LeadStatusHistoryId;
                }
            }

            return Result;
        }


        public bool CreateEMSLeadHistory(LeadHistory emsLeadHistory, LeadHistoryAction leadHistoryAction)
        {
            bool Result = false;
            LeadHistory historyToSave = leadHistoryAction == LeadHistoryAction.Update ? MergeEMSLeadHistory(emsLeadHistory) : emsLeadHistory;

            if (historyToSave != null)
            {
                Result = CreateEMSLeadHistory(historyToSave);
            }

            return Result;
        }

        private bool CreateEMSLeadHistory(LeadHistory emsLeadHistory)
        {
            bool Result = true;

            using (EMSModel Context = new EMSModel())
            {
                Context.LeadHistories.Attach(emsLeadHistory);
                Context.Entry(emsLeadHistory).State = EntityState.Added;
                Result = Context.SaveChanges() == 1;
            }

            return Result;
        }

        private LeadHistory MergeEMSLeadHistory(LeadHistory emsLeadHistory)
        {
            using (EMSModel Context = new EMSModel())
            {
                var lastHistory = (from lh in Context.LeadHistories
                                   where lh.LeadId == emsLeadHistory.LeadId
                                   orderby lh.LeadHistoryId descending
                                   select lh).FirstOrDefault();
                
                //Merge dictionaries
                if(lastHistory!=null && !string.IsNullOrWhiteSpace(lastHistory.JsonData))
                {
                    var newDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(emsLeadHistory.JsonData);
                    var oldDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(lastHistory.JsonData);
                    oldDictionary.Merge(newDictionary);
                    emsLeadHistory.JsonData = JsonConvert.SerializeObject(oldDictionary, Formatting.Indented);
                }
            }

            return emsLeadHistory;
        }

        public bool CreateEMSLeadActivities(List<LeadActivity> activities)
        {
            bool Result = true;

            using (EMSModel Context = new EMSModel())
            {
                foreach (var act in activities)
                {
                    Context.LeadActivities.Attach(act);
                    Context.Entry(act).State = EntityState.Added;
                }
                Result = Context.SaveChanges() == 1;
            }

            return Result;
        }

        private bool CreateLeadTransaction(EMSModel Context, LeadTransaction emsLeadTransaction)
        {
            bool Result = false;

            Context.LeadTransactions.Attach(emsLeadTransaction);
            Context.Entry(emsLeadTransaction).State = EntityState.Added;
            Result = Context.SaveChanges() == 1;
            return Result;
        }

        public bool CreateLeadTransaction(LeadTransaction emsLeadTransaction)
        {
            bool Result = false;

            using (EMSModel Context = new EMSModel())
            {
                Result = CreateLeadTransaction(Context, emsLeadTransaction);
            }
            return Result;
        }

        public long CreateLeadClientInfo(LeadClientInfo leadClientInfo)
        {
            long Result = 0;

            using (EMSModel Context = new EMSModel())
            {
                Context.LeadClientInfos.Attach(leadClientInfo);
                Context.Entry(leadClientInfo).State = EntityState.Added;
                Context.SaveChanges();
                Result = leadClientInfo.LeadClientInfoId;
            }

            return Result;
        }

        public long CreateLeadSalesforceInfo(LeadSalesforceInfo leadSalesforceInfo)
        {
            long Result = 0;

            using (EMSModel Context = new EMSModel())
            {
                Context.LeadSalesforceInfos.Attach(leadSalesforceInfo);
                Context.Entry(leadSalesforceInfo).State = EntityState.Added;
                Context.SaveChanges();
                Result = leadSalesforceInfo.LeadSalesforceInfoId;
            }

            return Result;
        }

        public long CreateLeadMarketingAttribution(LeadMarketingAttribution leadMarketingAttribution)
        {
            long Result = 0;

            using (EMSModel Context = new EMSModel())
            {
                Context.LeadMarketingAttributions.Attach(leadMarketingAttribution);
                Context.Entry(leadMarketingAttribution).State = EntityState.Added;
                Context.SaveChanges();
                Result = leadMarketingAttribution.LeadId;
            }

            return Result;
        }

        public bool UpdateLeadClientInfo(LeadClientInfo leadClientInfo)
        {
            bool Result = false;
            using (EMSModel Context = new EMSModel())
            {
                Context.LeadClientInfos.Attach(leadClientInfo);
                Context.Entry(leadClientInfo).State = EntityState.Modified;
                Result = Context.SaveChanges() == 1;
            }
            return Result;
        }

        public bool UpdateLeadSalesforceInfo(LeadSalesforceInfo leadSalesforceInfo)
        {
            bool Result = false;
            using (EMSModel Context = new EMSModel())
            {
                Context.LeadSalesforceInfos.Attach(leadSalesforceInfo);
                Context.Entry(leadSalesforceInfo).State = EntityState.Modified;
                Result = Context.SaveChanges() == 1;
            }
            return Result;
        }

        public LeadClientInfo GetLeadClientInfo(long leadId)
        {
            LeadClientInfo Result = null;

            using (EMSModel Context = new EMSModel())
            {
                var query = from l in Context.LeadClientInfos
                            where l.LeadId == leadId
                            select l;

                Result = query.FirstOrDefault();
            }

            return Result;
        }

        public LeadSalesforceInfo GetLeadSalesforceInfo(long leadId)
        {
            LeadSalesforceInfo Result = null;

            using (EMSModel Context = new EMSModel())
            {
                var query = from l in Context.LeadSalesforceInfos
                            where l.LeadId == leadId
                            select l;

                Result = query.FirstOrDefault();
            }

            return Result;
        }

        public bool CheckCampaignSendsToFBAPI(string campaignTrackId)
        {
            bool res = true;
            using (MarketingDataModel mdm = new MarketingDataModel())
            {
                Guid trackid = Guid.Parse(campaignTrackId);
                res = mdm.CampaignPixelMappings.Any(c => c.CampaignTrackId == trackid);
            }
            return res;
        }

        public bool CheckCampaignSendsEventToFBAPI(string campaignTrackId, int eventId)
        {
            bool res = true;
            using(MarketingDataModel mdm = new MarketingDataModel())
            {
                Guid trackid = Guid.Parse(campaignTrackId);
                res = (from cpm in mdm.CampaignPixelMappings.Where(c=>c.CampaignTrackId == trackid)
                          join p in mdm.Pixels on cpm.PixelId equals p.PixelId
                          join pen in mdm.PixelEventNames on p.PixelId equals pen.PixelId
                          where pen.ConversionAPIEventId == eventId && pen.IsEnabled
                           select pen).Count() > 0;
            }
            return res;
        }

        public bool CheckCampaignSendsToGoogleAPI(string campaignTrackId, int eventid)
        {
            bool res = true;
            using (MarketingDataModel mdm = new MarketingDataModel())
            {
                Guid trackid = Guid.Parse(campaignTrackId);
                res =  (from cpm in mdm.CampaignConversionMappings.Where(c => c.CampaignTrackId == trackid)
                        join p in mdm.Conversions on cpm.ConversionId equals p.ConversionId
                        join pen in mdm.ConversionEventNames on p.ConversionId equals pen.ConversionId
                        where pen.ConversionAPIEventId == eventid && pen.IsEnabled
                        select pen).Count() > 0;
            }
            return res;
        }

        public bool CheckInstitutionSendsToFBAudience(int institutionId)
        {
            bool res = true;
            using (MarketingDataModel mdm = new MarketingDataModel())
            {
                res = mdm.AudienceInstitutionMappings.Any(c => c.InstitutionId == institutionId && c.IsEnabled);
            }
            return res;
        }

        public VW_ProgramMapping GetEMSProgramMapping(int? programProductId)
        {
            VW_ProgramMapping res = new VW_ProgramMapping();
            if (programProductId != null)
            {
                using (EMSModel mdm = new EMSModel())
                {
                    res = mdm.VW_ProgramMappings.Where(pm => pm.ProgramProductId == programProductId).FirstOrDefault();
                }
            }
            return res;
        }
        public VW_ProgramMapping GetEMSProgramMapping(int? programProductId, int institutionId)
        {
            VW_ProgramMapping res = new VW_ProgramMapping();
            if (programProductId != null)
            {
                using (EMSModel mdm = new EMSModel())
                {
                    res = mdm.VW_ProgramMappings.Where(pm => pm.ProgramProductId == programProductId && pm.InstitutionId == institutionId).FirstOrDefault();
                }
            }
            return res;
        }

        public bool CheckInstitutionSendsToGoogleAudience(int institutionId)
        {
            bool res = true;

            using (MarketingDataModel mdm = new MarketingDataModel())
            {
                res = mdm.GAudienceInstitutionMappings.Any(c => c.InstitutionId == institutionId && c.IsEnabled);
            }

            return res;
        }


        public void GetLeadTimezoneData(Lead lead)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["EDDYAPI_URI"]);
                    string endpoint = $"{ConfigurationManager.AppSettings["TimezoneEndpoint"]}&PhoneNumber={lead.Phone1}&PostalCode={lead.PostalCode}&StateCode={lead.StateProvince}";
                    HttpResponseMessage response = client.GetAsync(endpoint).GetAwaiter().GetResult();
                    string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (responseBody != null)
                    {
                        TimezoneResponse tzResponse = JsonConvert.DeserializeObject<TimezoneResponse>(responseBody);
                        if (tzResponse.IsSuccessful)
                        {
                            lead.Timezone = tzResponse.Body.TimeZone;
                            lead.TimezoneOffset = tzResponse.Body.Offset;
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private class TimezoneResponse
        {
            public bool IsSuccessful { get; set; }
            public TimezoneResponseBody Body { get; set; }
            public class TimezoneResponseBody
            {
                public string TimeZone { get; set; }
                public string Offset { get; set; }
                public string FailureMessage { get; set; }

            }
        }

        public bool AllowSyncToSFBasedOnProgramStateRules(Lead lead)
        {
            VW_ProgramMapping programMapping = null;
            bool programStateAllowsSF = true;
            int stateId = 0;
            try
            {
                //Check whether Institution level enabled or not, if so fetch program-state rules config data.            
                bool programStateSyncConfigured = CacheDataService.InstitutionsWithProgramStateSFSync.Any(x => x == lead.InstitutionId);
                if (programStateSyncConfigured)
                {
                    if (lead.ISProgramProductId != null)
                        programMapping = GetEMSProgramMapping(lead.ISProgramProductId, lead.InstitutionId);

                    //Get StateID by zipcode and/or State Province
                    stateId = GetStateByZipCode(lead.PostalCode, lead.StateProvince) ?? 0;

                    if (programMapping != null && stateId != 0)
                    {
                        //Check Program-State combo allowed lead to send to SF
                        programStateAllowsSF = ProgramStateRuleAllowsSync(lead.InstitutionId, programMapping.ProgramId, stateId, lead.PostalCode);
                    }
                    //else                
                    //wrong zip code or config missing for Program - allowing SF sync

                }

            }
            catch (Exception e) { }
            return programStateAllowsSF;
        }

        public (ProgramStateSyncRule rule, List<ProgramStateSyncRuleState> ruleStates)
        GetProgramStateSyncData(int institutionId, int programId)
        {
            using (EMSModel Context = new EMSModel())
            {
                var rule = Context.ProgramStateSyncRules
                    .FirstOrDefault(x =>
                        x.InstitutionID == institutionId &&
                        x.ProgramID == programId &&
                        x.IsEnabled);

                if (rule == null)
                {
                    return (null, new List<ProgramStateSyncRuleState>());
                }

                // Skip details fetch for ALLOW_ALL / ALLOW_NONE
                if (rule.RuleType == ProgramStateSyncRuleType.ALLOW_ALL ||
                    rule.RuleType == ProgramStateSyncRuleType.ALLOW_NONE)
                {
                    return (rule, new List<ProgramStateSyncRuleState>());
                }
                var ruleStates = Context.ProgramStateSyncRuleStates
                    .Where(x =>
                        x.ProgramStateSyncRuleID == rule.ProgramStateSyncRuleID &&
                        x.IsEnabled)
                    .ToList();

                return (rule, ruleStates);
            }
        }

        public int? GetStateByZipCode(string zipCode, string stateCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
                return null;

            using (EMSModel Context = new EMSModel())
            {
                var zipCodeParam = new SqlParameter("@ZipCode", zipCode.Trim());

                var sql = @"SELECT ZIPCode, StateCode, StateID
                            FROM Nexus.Prod.USZipCityStateCountry WITH (NOLOCK)
                            WHERE ZIPCode = @ZipCode
                              AND StateID IS NOT NULL";

                var zipResults = Context.Database
                    .SqlQuery<ZipCodeStateResult>(sql, zipCodeParam)
                    .ToList();


                if (!zipResults.Any())
                    return null;

                ZipCodeStateResult selectedRecord = null;

                if (zipResults.Count == 1)
                {
                    selectedRecord = zipResults.First();
                }
                else
                {
                    selectedRecord = zipResults
                        .FirstOrDefault(x => x.StateCode == stateCode);

                    if (selectedRecord == null)
                    {
                        selectedRecord = zipResults.First();
                    }
                }

                return selectedRecord.StateID;
            }
        }
        public bool ProgramStateRuleAllowsSync(int institutionId, int programId, int stateId, string zipCode)
        {
            var rulesData = GetProgramStateSyncData(institutionId, programId);

            var rule = rulesData.rule;

            // If no rule found → allow sync
            if (rule == null)
                return true;

            if (rule.RuleType == ProgramStateSyncRuleType.ALLOW_ALL)
                return true;

            if (rule.RuleType == ProgramStateSyncRuleType.ALLOW_NONE)
                return false;

            // Get state configuration
            var stateConfig = rulesData.ruleStates.FirstOrDefault(rs =>
                rs.ProgramStateSyncRuleID == rule.ProgramStateSyncRuleID &&
                rs.StateID == stateId &&
                rs.IsEnabled);

            bool isStateAllowed = true;

            //state-level logic
            if (rule.RuleType == ProgramStateSyncRuleType.ALLOW_ONLY)
            {
                isStateAllowed = stateConfig != null;
            }
            else if (rule.RuleType == ProgramStateSyncRuleType.ALLOW_ALL_EXCEPT)
            {
                isStateAllowed = stateConfig == null;
            }

            // If state not allowed → stop
            if (!isStateAllowed)
                return false;

            // If no state config OR ZIP filtering not enabled → allow
            if (stateConfig == null || !stateConfig.ZipLevelSyncEnabled)
                return true;

            // ZIP filtering enabled → ZIP must match
            if (string.IsNullOrWhiteSpace(zipCode))
                return false;
            using (EMSModel Context = new EMSModel())
            {
                return Context.ProgramStateSyncRuleStateZips
                    .Any(z => z.ProgramStateSyncRuleStateID == stateConfig.ProgramStateSyncRuleStateID &&
                              z.ZipCode == zipCode &&
                              z.IsEnabled);
            }
        }
        
        public bool CanRoutePHEALeadBySource(int institutionId, string utmSource)
        {
            if(institutionId == 304)
            {
                var source = (utmSource ?? string.Empty).Trim().ToLower();

                bool isEddyCoachSource =
                    source == "facebook" ||
                    source == "fb" ||
                    source == "ig" ||
                    source == "instagram";

                // Do not route to SF for Institution 304 unless source is EddyCoach
                if (!isEddyCoachSource)
                    return false;
            }
            return true;
        }
    }
}
