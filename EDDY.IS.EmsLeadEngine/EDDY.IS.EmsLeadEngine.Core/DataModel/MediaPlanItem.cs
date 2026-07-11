using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    [Table("MediaPlanItem")]
    public partial class MediaPlanItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MediaPlanItem()
        {

        }

        public int MediaPlanItemId { get; set; }

        public int MediaPlanId { get; set; }

        public int ItemGPMappingId { get; set; }

        public int PricingModelId { get; set; }

        public Guid CampaignTrackId { get; set; }

        public int? EngagementId { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public decimal BudgetedSpend { get; set; }

        public bool OverwriteCurrentSpend { get; set; }

        public DateTime? LastCurrentSpendImportDate { get; set; }

        public int? CurrentBudgetedInquiries { get; set; }

        public decimal? CPL { get; set; }

        public int? DealId { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(128)]
        public string CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        [Required]
        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public decimal CurrentSpend { get; set; }

        public int? InitialBudgetedInquiries { get; set; }

        public bool? IsAllowedForAutoResponder { get; set; }
    }
}
