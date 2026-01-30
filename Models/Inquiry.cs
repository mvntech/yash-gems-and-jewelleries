using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yash_Gems___Jewelleries.Models
{
    /// <summary>
    /// Customer inquiry/contact form table
    /// </summary>
    [Table("Inquiries")]
    public class Inquiry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InquiryId { get; set; }

        // User reference (optional - can be null for guest inquiries)
        [StringLength(450)]
        public string? UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Inquiry Type")]
        public string? InquiryType { get; set; } // General, Product, Order, Support, Custom Design

        [StringLength(50)]
        [Display(Name = "Product Style Code")]
        public string? StyleCode { get; set; } // If inquiry is about specific product

        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "New"; // New, InProgress, Resolved, Closed

        [StringLength(2000)]
        [Display(Name = "Admin Response")]
        [DataType(DataType.MultilineText)]
        public string? AdminResponse { get; set; }

        [StringLength(450)]
        [Display(Name = "Responded By")]
        public string? RespondedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Response Date")]
        public DateTime? ResponseDate { get; set; }

        [Display(Name = "Priority")]
        public int Priority { get; set; } = 2; // 1=High, 2=Medium, 3=Low

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        [Display(Name = "IP Address")]
        public string? IpAddress { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey(nameof(StyleCode))]
        public virtual Item? Item { get; set; }
    }
}
