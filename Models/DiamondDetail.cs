using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yash_Gems___Jewelleries.Models.Masters;

namespace Yash_Gems___Jewelleries.Models
{
    /// <summary>
    /// Diamond details table - linked to Item
    /// Stores information about diamonds used in jewelry pieces
    /// </summary>
    [Table("DiamondDetails")]
    public class DiamondDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiamondDetailId { get; set; }

        // Foreign Key to Item
        [Required]
        [StringLength(50)]
        [Display(Name = "Style Code")]
        public string StyleCode { get; set; } = string.Empty;

        // Foreign Key to DiamondQuality
        [Required]
        [Display(Name = "Diamond Quality")]
        public int DiamondQualityId { get; set; }

        // Diamond Specifications
        [Required]
        [Column(TypeName = "decimal(10,3)")]
        [Display(Name = "Carat")]
        [Range(0, 9999.999)]
        public decimal Carat { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Number of Pieces")]
        public int Pieces { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,3)")]
        [Display(Name = "Weight (grams)")]
        [Range(0, 9999999.999)]
        public decimal Weight { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Rate per Carat")]
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

        [StringLength(500)]
        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(StyleCode))]
        public virtual Item Item { get; set; } = null!;

        [ForeignKey(nameof(DiamondQualityId))]
        public virtual DiamondQuality DiamondQuality { get; set; } = null!;

        /// <summary>
        /// Calculate the total amount based on carat and rate
        /// </summary>
        public void CalculateTotalAmount()
        {
            TotalAmount = Carat * Rate;
        }
    }
}
