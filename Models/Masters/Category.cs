using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yash_Gems___Jewelleries.Models;

namespace Yash_Gems___Jewelleries.Models.Masters
{
    /// <summary>
    /// Master table for jewelry categories (e.g., Gold, Diamond, Silver)
    /// </summary>
    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
