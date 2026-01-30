using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yash_Gems___Jewelleries.Models;

namespace Yash_Gems___Jewelleries.Models.Masters
{
    /// <summary>
    /// Master table for diamond quality grades (e.g., IF, VVS1, VVS2, VS1, VS2, SI1, SI2)
    /// </summary>
    [Table("DiamondQualities")]
    public class DiamondQuality
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiamondQualityId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Diamond Quality")]
        public string QualityGrade { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Clarity")]
        public string? Clarity { get; set; }

        [StringLength(50)]
        [Display(Name = "Color")]
        public string? Color { get; set; }

        [StringLength(50)]
        [Display(Name = "Cut")]
        public string? Cut { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<DiamondDetail> DiamondDetails { get; set; } = new List<DiamondDetail>();
    }
}
