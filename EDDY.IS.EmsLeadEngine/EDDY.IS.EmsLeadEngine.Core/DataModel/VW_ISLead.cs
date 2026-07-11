namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VW_ISLead
    {
        [Key]
        [Column(Order = 0, TypeName = "numeric")]
        public decimal LeadID { get; set; }

        [StringLength(64)]
        public string FirstName { get; set; }

        [StringLength(64)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string EmailAddress { get; set; }

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
        public string ZipCode { get; set; }

        public int? ClientRelationshipId { get; set; }

        [StringLength(18)]
        public string InstitutionSalesForceId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InstitutionId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(255)]
        public string InstitutionName { get; set; }

        public int? ProgramProductId { get; set; }

        public int? ProgramLevelId { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(64)]
        public string ProgramLevelName { get; set; }

        [StringLength(50)]
        public string EducationLevelId { get; set; }

        public int? SubmissionId { get; set; }

        public Guid? TrackId { get; set; }

        [StringLength(255)]
        public string utm_campaign { get; set; }

        [StringLength(255)]
        public string utm_channel { get; set; }

        [StringLength(255)]
        public string utm_vendor { get; set; }

        [StringLength(50)]
        public string ExternalId { get; set; }

        public int? MediaPlanItemId { get; set; }

        public decimal? CPL { get; set; }

        public string AdditionalFields { get; set; }

        public string MethodOfContact { get; set; }

        public string InitialLeadValidationFailed { get; set; }

        public int CampusTypeId { get; set; }
        public int CampusId { get; set; }
        public int ProgramId { get; set; }

        [StringLength(255)]
        public string InitialLeadValidationFailedReason { get; set; }

        public string ReadyToStart { get; set; }

        [StringLength(255)]
        public string SourceCode { get; set; }

        public string RawPostBrowserInfo { get; set; }

        public string RawPostReferer { get; set; }
        public string RawPostIP { get; set; }

        public string FormLeadUrl { get; set; }

        public string LeadUserAgent { get; set; }
        public string IPAddress { get; set; }
        public string EMSContractName { get; set; }

    }
}
