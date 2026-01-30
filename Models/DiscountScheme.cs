using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yash_Gems___Jewelleries.Models
{
    /// <summary>
    /// Discount schemes and promotional offers
    /// </summary>
    [Table("DiscountSchemes")]
    public class DiscountScheme
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscountSchemeId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Scheme Name")]
        public string SchemeName { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Discount Type")]
        public string DiscountType { get; set; } = "Percentage"; // Percentage, FixedAmount

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Discount Value")]
        [Range(0, 999999999999.99)]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Minimum Purchase Amount")]
        public decimal? MinimumPurchaseAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Maximum Discount Amount")]
        public decimal? MaximumDiscountAmount { get; set; }

        [StringLength(50)]
        [Display(Name = "Coupon Code")]
        public string? CouponCode { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Usage Limit")]
        public int? UsageLimit { get; set; }

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; } = 0;

        [Display(Name = "Per User Limit")]
        public int? PerUserLimit { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Applicable On")]
        public string ApplicableOn { get; set; } = "All"; // All, Category, Brand, ProductType, SpecificProducts

        [StringLength(500)]
        [Display(Name = "Applicable IDs")]
        public string? ApplicableIds { get; set; } // Comma-separated IDs

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Check if the discount scheme is currently valid
        /// </summary>
        public bool IsValid()
        {
            DateTime now = DateTime.UtcNow;
            return IsActive && 
                   now >= StartDate && 
                   now <= EndDate &&
                   (UsageLimit == null || UsageCount < UsageLimit);
        }
    }
}
