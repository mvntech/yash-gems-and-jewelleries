using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yash_Gems___Jewelleries.Models
{
    /// <summary>
    /// Product review and rating table
    /// </summary>
    [Table("Reviews")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        // Foreign Key to Item
        [Required]
        [StringLength(50)]
        [Display(Name = "Style Code")]
        public string StyleCode { get; set; } = string.Empty;

        // Foreign Key to User
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        [Display(Name = "Rating")]
        public int Rating { get; set; } // 1 to 5 stars

        [Required]
        [StringLength(200)]
        [Display(Name = "Review Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Review Content")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Is Verified Purchase")]
        public bool IsVerifiedPurchase { get; set; } = false;

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; } = false;

        [Display(Name = "Helpful Count")]
        public int HelpfulCount { get; set; } = 0;

        [Display(Name = "Not Helpful Count")]
        public int NotHelpfulCount { get; set; } = 0;

        [StringLength(2000)]
        [Display(Name = "Vendor Response")]
        [DataType(DataType.MultilineText)]
        public string? VendorResponse { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Response Date")]
        public DateTime? ResponseDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey(nameof(StyleCode))]
        public virtual Item Item { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
