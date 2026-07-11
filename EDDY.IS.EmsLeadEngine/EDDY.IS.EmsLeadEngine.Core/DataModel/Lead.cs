namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Lead")]
    public partial class Lead
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lead()
        {
            LeadActivities = new HashSet<LeadActivity>();
            LeadHistories = new HashSet<LeadHistory>();
            LeadStatusHistories = new HashSet<LeadStatusHistory>();
        }

        public long LeadId { get; set; }

        public Guid LeadGUID { get; set; }

        public long PersonId { get; set; }

        public int InstitutionId { get; set; }

        public int LeadSourceTypeId { get; set; }

        public int? MediaPlanItemId { get; set; }

        public decimal? CPL { get; set; }

        public int? EngagementId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ISLeadId { get; set; }

        [StringLength(18)]
        public string SalesforceId { get; set; }

        [StringLength(18)]
        public string LegacyGPLeadId { get; set; }

        [StringLength(50)]
        public string ExternalId { get; set; }

        [StringLength(64)]
        public string FirstName { get; set; }

        [StringLength(64)]
        public string LastName { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone1 { get; set; }

        [StringLength(50)]
        public string Phone2 { get; set; }

        [StringLength(100)]
        public string Address1 { get; set; }

        [StringLength(100)]
        public string Address2 { get; set; }

        [StringLength(64)]
        public string City { get; set; }

        [StringLength(50)]
        public string StateProvince { get; set; }

        [StringLength(50)]
        public string CountryCode { get; set; }

        [StringLength(25)]
        public string PostalCode { get; set; }

        public int? ISProgramProductId { get; set; }

        public int? EducationLevelId { get; set; }

        public string StartTerm { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public DateTime? SFUpdatedDate { get; set; }

        public int LeadStatusId { get; set; }

        public int? LeadSubStatusId { get; set; }

        [StringLength(100)]
        public string ClosedReasonCode { get; set; }

        public int LeadStateId { get; set; }

        [StringLength(100)]
        public string LeadOwner { get; set; }

        public bool IsScrub { get; set; }

        public bool IsTest { get; set; }

        [StringLength(200)]
        public string utm_channel { get; set; }

        [StringLength(200)]
        public string utm_vendor { get; set; }

        [StringLength(200)]
        public string utm_campaign { get; set; }

        public string Notes { get; set; }

        public bool HasOptedOutOfEmail { get; set; }
        public bool? IsStealthApp { get; set; }
        public string RewarmingIndicator { get; set; }
        public string AppDialOutcome { get; set; }
        public string RewarmingDialOutcome { get; set; }
        public double? RewarmingContactAttempts { get; set; }
        public bool? HasOptedOutOfSMS { get; set; }
        public string Timezone { get; set; }
        public string TimezoneOffset { get; set; }
        public virtual Institution Institution { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeadActivity> LeadActivities { get; set; }

        public virtual LeadSourceType LeadSourceType { get; set; }

        public virtual LeadState LeadState { get; set; }

        public virtual LeadStatus LeadStatus { get; set; }

        public virtual LeadSubStatus LeadSubStatus { get; set; }

        public virtual Person Person { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeadHistory> LeadHistories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeadStatusHistory> LeadStatusHistories { get; set; }
    }
}
