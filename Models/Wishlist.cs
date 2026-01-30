using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yash_Gems___Jewelleries.Models
{
    /// <summary>
    /// User wishlist table for saved products
    /// </summary>
    [Table("Wishlists")]
    public class Wishlist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WishlistId { get; set; }

        // Foreign Key to User
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        // Foreign Key to Item
        [Required]
        [StringLength(50)]
        [Display(Name = "Style Code")]
        public string StyleCode { get; set; } = string.Empty;

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(StyleCode))]
        public virtual Item Item { get; set; } = null!;
    }
}
