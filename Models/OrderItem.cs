using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yash_Gems___Jewelleries.Models
{
    /// <summary>
    /// Order line items table - stores individual products in each order
    /// </summary>
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }

        // Foreign Key to Order
        [Required]
        [Display(Name = "Order")]
        public int OrderId { get; set; }

        // Foreign Key to Item
        [Required]
        [StringLength(50)]
        [Display(Name = "Style Code")]
        public string StyleCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Discount Percentage")]
        public decimal DiscountPercentage { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        // Product snapshot at time of order (for historical reference)
        [StringLength(100)]
        [Display(Name = "Brand")]
        public string? BrandName { get; set; }

        [StringLength(100)]
        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        [StringLength(100)]
        [Display(Name = "Product Type")]
        public string? ProductTypeName { get; set; }

        [StringLength(20)]
        [Display(Name = "Gold Carat")]
        public string? GoldCarat { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        [Display(Name = "Gold Weight")]
        public decimal? GoldWeight { get; set; }

        [StringLength(500)]
        [Display(Name = "Product Image")]
        public string? ProductImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey(nameof(StyleCode))]
        public virtual Item Item { get; set; } = null!;

        /// <summary>
        /// Calculate subtotal for this order item
        /// </summary>
        public void CalculateSubtotal()
        {
            decimal baseAmount = Quantity * UnitPrice;
            DiscountAmount = (baseAmount * DiscountPercentage) / 100;
            Subtotal = baseAmount - DiscountAmount + TaxAmount;
        }
    }
}
