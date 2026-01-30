using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yash_Gems___Jewelleries.Models
{
    /// <summary>
    /// Newsletter subscription table
    /// </summary>
    [Table("NewsletterSubscriptions")]
    public class NewsletterSubscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubscriptionId { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Display(Name = "Is Subscribed")]
        public bool IsSubscribed { get; set; } = true;

        [Display(Name = "Is Verified")]
        public bool IsVerified { get; set; } = false;

        [StringLength(100)]
        [Display(Name = "Verification Token")]
        public string? VerificationToken { get; set; }

        public DateTime SubscribedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        [Display(Name = "Unsubscribed Date")]
        public DateTime? UnsubscribedDate { get; set; }

        [StringLength(50)]
        [Display(Name = "IP Address")]
        public string? IpAddress { get; set; }

        [StringLength(200)]
        [Display(Name = "Source")]
        public string? Source { get; set; } // Homepage, Checkout, Footer, etc.
    }
}
