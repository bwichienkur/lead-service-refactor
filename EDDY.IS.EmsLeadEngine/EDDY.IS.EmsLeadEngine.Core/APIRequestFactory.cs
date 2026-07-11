using EDDY.IS.EmsLeadEngine.Core.DataModel;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Entities.AzureFunction;

using System;
using System.Configuration;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public static class APIRequestFactory
    {
        private static void GetMainAPIRequestInfo(BaseAPIRequest req, Lead EMSLead, VW_ISLead ISLead, Constants.ConversionAPIEvent ev, DateTime evDate)
        {
            string url = String.Empty, useragent = String.Empty, ip = String.Empty;
            if (ISLead != null)
            {
                url = string.IsNullOrEmpty(ISLead.FormLeadUrl) ? ISLead.RawPostReferer : ISLead.FormLeadUrl;
                useragent = string.IsNullOrEmpty(ISLead.LeadUserAgent) ? ISLead.RawPostBrowserInfo : ISLead.LeadUserAgent;
                ip = string.IsNullOrEmpty(ISLead.IPAddress) ? ISLead.RawPostIP : ISLead.IPAddress;
            }
            req.City = EMSLead.City;
            req.Country = EMSLead.CountryCode;
            req.Email = EMSLead.Email;
            req.Event = ev;
            req.EventDate = evDate;
            req.FirstName = EMSLead.FirstName;
            req.LastName = EMSLead.LastName;
            req.Phone = EMSLead.Phone1;
            req.State = EMSLead.StateProvince;
            req.Zip = EMSLead.PostalCode;
            req.ClientUserAgent = useragent;
            req.SourceUrl = url;
            req.ActionSource = "website";
            req.IPAddress = ip;
        }

        public static FBAudienceRequest GetFBAudienceRequest(Lead EMSLead, VW_ISLead ISLead, Constants.ConversionAPIEvent ev, DateTime evDate, VW_ProgramMapping programMapping)
        {
            long? nexusLeadId = null;
            if (EMSLead.ISLeadId != null)
                nexusLeadId = long.Parse(EMSLead.ISLeadId.GetValueOrDefault().ToString());

            FBAudienceRequest req = new FBAudienceRequest
            {
                CampusTypeId = EMSLead.ISLeadId != null ? ISLead?.CampusTypeId : programMapping?.CampusTypeId,
                CampusId = EMSLead.ISLeadId != null ? ISLead?.CampusId : programMapping?.CampusId,
                ProgramId = EMSLead.ISLeadId != null ? ISLead?.ProgramId : programMapping?.ProgramId,
                ProgramLevelId = EMSLead.ISLeadId != null ? ISLead?.ProgramLevelId : programMapping?.ProgramLevelId,
                InstitutionId = EMSLead.InstitutionId,
                StatusId = EMSLead.LeadStatusId,
                SubStatusId = EMSLead.LeadSubStatusId,
                ISLeadId = nexusLeadId,
                EMSLeadId = EMSLead.LeadId
            };

            GetMainAPIRequestInfo(req, EMSLead, ISLead, ev, evDate);

            return req;
        }

        public static GoogleAudienceAPIRequest GetGoogleAudienceRequest(Lead EMSLead, VW_ISLead ISLead, Constants.ConversionAPIEvent ev, DateTime evDate, VW_ProgramMapping programMapping)
        {
            long? nexusLeadId = null;
            if (EMSLead.ISLeadId != null)
                nexusLeadId = long.Parse(EMSLead.ISLeadId.GetValueOrDefault().ToString());

            GoogleAudienceAPIRequest req = new GoogleAudienceAPIRequest
            {
                CampusTypeId = EMSLead.ISLeadId != null ? ISLead?.CampusTypeId : programMapping?.CampusTypeId,
                CampusId = EMSLead.ISLeadId != null ? ISLead?.CampusId : programMapping?.CampusId,
                ProgramId = EMSLead.ISLeadId != null ? ISLead?.ProgramId : programMapping?.ProgramId,
                ProgramLevelId = EMSLead.ISLeadId != null ? ISLead?.ProgramLevelId : programMapping?.ProgramLevelId,
                InstitutionId = EMSLead.InstitutionId,
                StatusId = EMSLead.LeadStatusId,
                SubStatusId = EMSLead.LeadSubStatusId,
                ISLeadId = nexusLeadId,
                EMSLeadId = EMSLead.LeadId
            };

            GetMainAPIRequestInfo(req, EMSLead, ISLead, ev, evDate);

            return req;
        }

        public static FBConversionAPIRequest GetFBConversionAPIRequest(Lead EMSLead, VW_ISLead ISLead, Constants.ConversionAPIEvent ev, string trackid, DateTime evDate)
        {
            long? nexusLeadId = null;
            if (EMSLead.ISLeadId != null)
                nexusLeadId = long.Parse(EMSLead.ISLeadId.GetValueOrDefault().ToString());

            FBConversionAPIRequest req = new FBConversionAPIRequest()
            {
                CampaignTrackId = trackid,
                EMSLeadId = EMSLead.LeadId,
                ISLeadId = nexusLeadId
            };
            
            GetMainAPIRequestInfo(req, EMSLead, ISLead, ev, evDate);

            return req;
        }

        public static GoogleConversionAPIRequest GetGoogleConversionAPIRequest(Lead EMSLead, VW_ISLead ISLead, Constants.ConversionAPIEvent ev, string trackid, DateTime evDate)
        {
            long? nexusLeadId = null;
            if (EMSLead.ISLeadId != null)
                nexusLeadId = long.Parse(EMSLead.ISLeadId.GetValueOrDefault().ToString());

            GoogleConversionAPIRequest req = new GoogleConversionAPIRequest()
            {
                CampaignTrackId = trackid,
                EMSLeadId = EMSLead.LeadId,
                ISLeadId = nexusLeadId
            };

            GetMainAPIRequestInfo(req, EMSLead, ISLead, ev, evDate);

            return req;
        }
    }
}
