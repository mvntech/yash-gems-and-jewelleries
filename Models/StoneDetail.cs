using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yash_Gems___Jewelleries.Models.Masters;

namespace Yash_Gems___Jewelleries.Models
{
    /// <summary>
    /// Stone details table - linked to Item
    /// Stores information about gemstones (Ruby, Emerald, Sapphire, etc.) used in jewelry
    /// </summary>
    [Table("StoneDetails")]
    public class StoneDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoneDetailId { get; set; }

        // Foreign Key to Item
        [Required]
        [StringLength(50)]
        [Display(Name = "Style Code")]
        public string StyleCode { get; set; } = string.Empty;

        // Foreign Key to StoneQuality
        [Required]
        [Display(Name = "Stone Quality")]
        public int StoneQualityId { get; set; }

        // Stone Specifications
        [Required]
        [Column(TypeName = "decimal(10,3)")]
        [Display(Name = "Weight (grams)")]
        [Range(0, 9999999.999)]
        public decimal Weight { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Number of Pieces")]
        public int Pieces { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        [Display(Name = "Carat")]
        [Range(0, 9999.999)]
        public decimal Carat { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Rate per Piece/Carat")]
        [Range(0, 999999999999.99)]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [StringLength(100)]
        [Display(Name = "Shape")]
        public string? Shape { get; set; }

        [StringLength(100)]
        [Display(Name = "Setting Type")]
        public string? SettingType { get; set; }

        [StringLength(50)]
        [Display(Name = "Treatment")]
        public string? Treatment { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(StyleCode))]
        public virtual Item Item { get; set; } = null!;

        [ForeignKey(nameof(StoneQualityId))]
        public virtual StoneQuality StoneQuality { get; set; } = null!;

        /// <summary>
        /// Calculate the total amount based on weight/carat and rate
        /// </summary>
        public void CalculateTotalAmount()
        {
            // If carat is provided, use carat-based calculation
            if (Carat > 0)
            {
                TotalAmount = Carat * Rate;
            }
            else
            {
                // Otherwise use piece-based calculation
                TotalAmount = Pieces * Rate;
            }
        }
    }
}
