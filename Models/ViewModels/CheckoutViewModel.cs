using System.ComponentModel.DataAnnotations;
using Yash_Gems___Jewelleries.Models.ViewModels;

namespace Yash_Gems___Jewelleries.Models.ViewModels
{
    public class CheckoutViewModel
    {
        // Customer Information
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email or Phone is required")]
        [Display(Name = "Email or Phone")]
        public string EmailOrPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; } = string.Empty;

        // Shipping Address
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;

        public string? Apartment { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        public string? State { get; set; }

        [Required(ErrorMessage = "Zipcode is required")]
        public string ZipCode { get; set; } = string.Empty;

        // Shipping Method
        public string ShippingMethod { get; set; } = "Free"; // Free, Express

        // Payment Method
        public string PaymentMethod { get; set; } = "Credit Card"; // BankTransfer, COD, CreditCard, PayPal

        public bool UseShippingAsBilling { get; set; } = true;

        // Cart Summary
        public CartViewModel Cart { get; set; } = new CartViewModel();

        // For Credit Card Mock
        [Display(Name = "Card Number")]
        [DataType(DataType.CreditCard)]
        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Invalid Card Number format")]
        public string? CardNumber { get; set; }

        [Display(Name = "Expiration Date")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Invalid Expiration Date format (MM/YY)")]
        public string? ExpiryDate { get; set; }

        [Display(Name = "Security Code (CVV)")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVV format")]
        public string? CVV { get; set; }

        [Display(Name = "Name on Card")]
        [StringLength(100, ErrorMessage = "Card holder name too long")]
        public string? CardHolderName { get; set; }
    }
}
