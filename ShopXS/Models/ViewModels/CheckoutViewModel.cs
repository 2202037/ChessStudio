using System.ComponentModel.DataAnnotations;

namespace ShopXS.Models.ViewModels;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(200)]
    [Display(Name = "Full Name")]
    public string ShippingName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(500)]
    [Display(Name = "Address")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal code is required")]
    [StringLength(20)]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone]
    [Display(Name = "Phone Number")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = string.Empty;

    // Cart summary (read-only)
    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}
