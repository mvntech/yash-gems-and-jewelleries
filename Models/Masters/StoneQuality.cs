using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yash_Gems___Jewelleries.Models;

namespace Yash_Gems___Jewelleries.Models.Masters
{
    /// <summary>
    /// Master table for gemstone quality (e.g., Ruby, Emerald, Sapphire qualities)
    /// </summary>
    [Table("StoneQualities")]
    public class StoneQuality
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoneQualityId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Stone Type")]
        public string StoneType { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Quality Grade")]
        public string? QualityGrade { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [StringLength(50)]
        public string? Origin { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<StoneDetail> StoneDetails { get; set; } = new List<StoneDetail>();
    }
}
