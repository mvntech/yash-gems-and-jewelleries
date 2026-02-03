using System.ComponentModel.DataAnnotations;

namespace Yash_Gems___Jewelleries.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        
        [Display(Name = "Subtotal")]
        public decimal Subtotal => Items.Sum(i => i.Subtotal);
        
        [Display(Name = "Shipping")]
        public decimal ShippingCost { get; set; } = 0;
        
        [Display(Name = "Tax")]
        public decimal Tax { get; set; } = 0;
        
        [Display(Name = "Total")]
        public decimal Total => Subtotal + ShippingCost + Tax;
    }

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public string StyleCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
        public int MaxStock { get; set; }
    }
}
