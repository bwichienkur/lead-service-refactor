namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using EDDY.IS.EmsLeadEngine.Entities.Common;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using static EDDY.IS.EmsLeadEngine.Entities.Constants;

    public partial class EMSModel : DbContext
    {
        public EMSModel()
            : base("name=EMSModel")
        {
        }

        public virtual DbSet<Institution> Institutions { get; set; }
        public virtual DbSet<Lead> Leads { get; set; }
        public virtual DbSet<LeadActivity> LeadActivities { get; set; }
        public virtual DbSet<LeadActivityType> LeadActivityTypes { get; set; }
        public virtual DbSet<LeadHistory> LeadHistories { get; set; }
        public virtual DbSet<LeadSourceType> LeadSourceTypes { get; set; }
        public virtual DbSet<LeadState> LeadStates { get; set; }
        public virtual DbSet<LeadStatus> LeadStatuses { get; set; }
        public virtual DbSet<LeadStatusHistory> LeadStatusHistories { get; set; }
        public virtual DbSet<LeadSubStatus> LeadSubStatuses { get; set; }
        public virtual DbSet<LeadTransaction> LeadTransactions { get; set; }
        public virtual DbSet<MediaPlanItem> MediaPlanItems { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<LeadClientInfo> LeadClientInfos { get; set; }
        public virtual DbSet<LeadSalesforceInfo> LeadSalesforceInfos { get; set; }
        public virtual DbSet<VW_ISLead> VW_ISLeads { get; set; }
        public virtual DbSet<VW_ProgramMapping> VW_ProgramMappings { get; set; }
        public virtual DbSet<VW_SalesforceInstitutionProgramProductMapping> VW_SalesforceInstitutionProgramProductMappings { get; set; }
        public virtual DbSet<VW_ContractInstitutionsRules> VW_ContractInstitutionsRules { get; set; }
        public virtual DbSet<VW_EmailIntegrationInstitutions> VW_EmailIntegrationInstitutions { get; set; }
        public virtual DbSet<LeadMarketingAttribution> LeadMarketingAttributions { get; set; }
        public virtual DbSet<CustomEventInstitutionMapping> CustomEventInstitutionMappings { get; set; }
        public virtual DbSet<ProgramStateSyncRule> ProgramStateSyncRules { get; set; }
        public virtual DbSet<ProgramStateSyncRuleState> ProgramStateSyncRuleStates { get; set; }
        public virtual DbSet<ProgramStateSyncRuleStateZip> ProgramStateSyncRuleStateZips { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Institution>()
                .Property(e => e.SalesforceId)
                .IsFixedLength();

            modelBuilder.Entity<Institution>()
                .Property(e => e.InstitutionName)
                .IsUnicode(false);

            modelBuilder.Entity<Institution>()
                .Property(e => e.InstitutionShortName)
                .IsUnicode(false);

            modelBuilder.Entity<Institution>()
                .HasMany(e => e.Leads)
                .WithRequired(e => e.Institution)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.ISLeadId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Lead>()
                .Property(e => e.SalesforceId)
                .IsFixedLength();

            modelBuilder.Entity<Lead>()
                .Property(e => e.LegacyGPLeadId)
                .IsFixedLength();

            modelBuilder.Entity<Lead>()
                .Property(e => e.ExternalId)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Phone1)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Phone2)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Address1)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.Address2)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.StateProvince)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.CountryCode)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.PostalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.ClosedReasonCode)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.LeadOwner)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.utm_channel)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.utm_vendor)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .Property(e => e.utm_campaign)
                .IsUnicode(false);

            modelBuilder.Entity<Lead>()
                .HasMany(e => e.LeadActivities)
                .WithRequired(e => e.Lead)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .HasMany(e => e.LeadHistories)
                .WithRequired(e => e.Lead)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .HasMany(e => e.LeadStatusHistories)
                .WithRequired(e => e.Lead)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .Property(x => x.CPL)
                .HasPrecision(18, 10);

            modelBuilder.Entity<LeadActivity>()
                .Property(e => e.SalesforceLastModifiedDate)
                .IsUnicode(false);

            modelBuilder.Entity<LeadActivityType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<LeadActivityType>()
                .HasMany(e => e.LeadActivities)
                .WithRequired(e => e.LeadActivityType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadSourceType>()
                .Property(e => e.LeadSourceTypeName)
                .IsUnicode(false);

            modelBuilder.Entity<LeadSourceType>()
                .HasMany(e => e.Leads)
                .WithRequired(e => e.LeadSourceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadState>()
                .Property(e => e.LeadStateName)
                .IsUnicode(false);

            modelBuilder.Entity<LeadState>()
                .HasMany(e => e.Leads)
                .WithRequired(e => e.LeadState)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadStatus>()
                .Property(e => e.LeadStatusName)
                .IsUnicode(false);

            modelBuilder.Entity<LeadStatus>()
                .HasMany(e => e.Leads)
                .WithRequired(e => e.LeadStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadStatus>()
                .HasMany(e => e.LeadStatusHistories)
                .WithRequired(e => e.LeadStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadStatus>()
                .HasMany(e => e.LeadSubStatus)
                .WithRequired(e => e.LeadStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadStatusHistory>()
                .Property(e => e.ClosedReasonCode)
                .IsUnicode(false);

            modelBuilder.Entity<LeadStatusHistory>()
                .HasMany(e => e.LeadActivities)
                .WithRequired(e => e.LeadStatusHistory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadTransaction>()
                .Property(e => e.Action)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Leads)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadClientInfo>()
                .Property(e => e.InitialStartTerm)
                .IsUnicode(false);

            modelBuilder.Entity<LeadClientInfo>()
                .Property(e => e.ApplicationStartTerm)
                .IsUnicode(false);

            modelBuilder.Entity<LeadClientInfo>()
                .Property(e => e.ApplicationDegreeName)
                .IsUnicode(false);

            modelBuilder.Entity<LeadClientInfo>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<LeadClientInfo>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.LeadID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.EmailAddress)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.Phone1)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.Phone2)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.Address1)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.Address2)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.StateProvince)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.CountryCode)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.ZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.InstitutionSalesForceId)
                .IsFixedLength();

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.InstitutionName)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.ProgramLevelName)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.EducationLevelId)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.utm_campaign)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.utm_channel)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.utm_vendor)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.InitialLeadValidationFailedReason)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ISLead>()
                .Property(e => e.MethodOfContact)
                .IsUnicode(false);

            modelBuilder.Entity<VW_SalesforceInstitutionProgramProductMapping>()
                .Property(e => e.InstitutionSalesforceId)
                .IsFixedLength();

            modelBuilder.Entity<VW_SalesforceInstitutionProgramProductMapping>()
                .Property(e => e.ProgramProductSalesforceId)
                .IsUnicode(false);

            modelBuilder.Entity<VW_ContractInstitutionsRules>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<ProgramStateSyncRule>()
                .ToTable("ProgramStateSyncRule");

            modelBuilder.Entity<ProgramStateSyncRuleState>()
                .ToTable("ProgramStateSyncRuleState");
            modelBuilder.Entity<ProgramStateSyncRuleStateZip>()
                .ToTable("ProgramStateSyncRuleStateZip");
        }
    }
}
