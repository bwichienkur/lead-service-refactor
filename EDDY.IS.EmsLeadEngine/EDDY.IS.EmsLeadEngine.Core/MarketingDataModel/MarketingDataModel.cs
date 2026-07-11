namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MarketingDataModel : DbContext
    {
        public MarketingDataModel()
            : base("name=MarketingDataModel")
        {
        }

        public virtual DbSet<AudienceInstitutionMapping> AudienceInstitutionMappings { get; set; }
        public virtual DbSet<AudienceLeadEvent> AudienceLeadEvents { get; set; }
        public virtual DbSet<CampaignPixelMapping> CampaignPixelMappings { get; set; }
        public virtual DbSet<CampaignConversionMapping> CampaignConversionMappings { get; set; }
        public virtual DbSet<Conversion> Conversions { get; set; }
        public virtual DbSet<ConversionEventName> ConversionEventNames { get; set; }
        public virtual DbSet<LeadConversionAPIEvent> LeadConversionAPIEvents { get; set; }
        public virtual DbSet<GLeadConversionAPIEvent> GLeadConversionAPIEvents { get; set; }
        public virtual DbSet<GAudienceLeadEvent> GAudienceLeadEvents { get; set; }
        public virtual DbSet<GAudienceInstitutionMapping> GAudienceInstitutionMappings { get; set; }
        public virtual DbSet<Pixel> Pixels { get; set; }
        public virtual DbSet<PixelEventName> PixelEventNames { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
